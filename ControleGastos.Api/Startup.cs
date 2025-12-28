using ControleGastos.Application.Interfaces;
using ControleGastos.Application.Services;
using ControleGastos.Domain.Interfaces.Repositories;
using ControleGastos.Infrastructure;
using ControleGastos.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


namespace ControleGastos.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Azure AD Authentication for multi-tenant + personal accounts
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = "https://login.microsoftonline.com/common/v2.0";
                        options.Audience = Configuration["AzureAd:ClientId"];
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false, // Required for personal accounts from different tenants
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidAudience = Configuration["AzureAd:ClientId"]
                        };

                        // Enhanced logging for debugging
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var token = context.Request.Headers["Authorization"].ToString();
                                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                                logger.LogInformation($"🔑 Token received: {(!string.IsNullOrEmpty(token) ? "YES" : "NO")}");
                                logger.LogInformation($"📍 Request Path: {context.Request.Path}");
                                logger.LogInformation($"🌐 Origin: {context.Request.Headers["Origin"]}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                                logger.LogInformation("✅ Token validated successfully");
                                
                                var claims = context.Principal?.Claims;
                                if (claims != null)
                                {
                                    foreach (var claim in claims.Take(10))
                                    {
                                        logger.LogInformation($"   Claim: {claim.Type} = {claim.Value}");
                                    }
                                }
                                return Task.CompletedTask;
                            },
                            OnAuthenticationFailed = context =>
                            {
                                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                                logger.LogError($"❌ Authentication failed: {context.Exception.Message}");
                                logger.LogError($"   Exception Type: {context.Exception.GetType().Name}");
                                
                                if (context.Exception.InnerException != null)
                                {
                                    logger.LogError($"   Inner Exception: {context.Exception.InnerException.Message}");
                                }
                                
                                return Task.CompletedTask;
                            },
                            OnChallenge = context =>
                            {
                                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                                logger.LogWarning($"⚠️ Challenge triggered");
                                logger.LogWarning($"   Error: {context.Error}");
                                logger.LogWarning($"   ErrorDescription: {context.ErrorDescription}");
                                logger.LogWarning($"   AuthenticateFailure: {context.AuthenticateFailure?.Message}");
                                return Task.CompletedTask;
                            },
                            OnForbidden = context =>
                            {
                                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                                logger.LogWarning("🚫 Access forbidden (403)");
                                logger.LogWarning($"   User: {context.HttpContext.User?.Identity?.Name ?? "Anonymous"}");
                                logger.LogWarning($"   IsAuthenticated: {context.HttpContext.User?.Identity?.IsAuthenticated}");
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddAuthorization();

            services.AddDbContext<ControleGastosDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configure CORS
            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" };

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPessoaRepository, PessoaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ITransacaoService, TransacaoService>();

            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "ControleGastos API", 
                    Version = "v1",
                    Description = "API para controle de gastos pessoais",
                    Contact = new OpenApiContact
                    {
                        Name = "ControleGastos",
                        Url = new Uri("https://github.com/IvanNaka/ControleGastosAPI")
                    }
                });

                // Configure Swagger to use Bearer token
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleGastos API v1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
