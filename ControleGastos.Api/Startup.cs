using ControleGastos.Infrastructure;
using ControleGastos.Infrastructure.Repositories;
using ControleGastos.Application.Interfaces;
using ControleGastos.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ControleGastos.Domain.Interfaces.Repositories;


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
            services.AddAuthorization();

            services.AddDbContext<ControleGastosDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Configure CORS
            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

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
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleGastos API v1");
                });
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
