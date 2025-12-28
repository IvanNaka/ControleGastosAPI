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
                            ValidAudience = Configuration["AzureAd:ClientId"],
                            // Accept tokens from any Microsoft tenant
                            ValidIssuers = new[]
                            {
                                "https://login.microsoftonline.com/common/v2.0",
                                "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0"
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


            app.UseDeveloperExceptionPage();


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
