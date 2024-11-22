using Carter;
using Core.Feature;
using Core.Helpers;
using Core.Helpers.Env;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core;

public class CoreFeature : IFeature
{
    public void ConfigureBuilder(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsProduction())
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
            });
        }

        builder.Configuration.AddEnvFile(".env");
    }

    public void ConfigureApp(WebApplication app)
    {
        app.MapGroup("api/v1").MapCarter();
        app.UseCors(AppConstants.CorsPolicy);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AddWebApiConfig(services);
    }

    private static void AddWebApiConfig(IServiceCollection services)
    {
        services.AddCarter();

        services.AddCors(options =>
        {
            options.AddPolicy(name: AppConstants.CorsPolicy,
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "compozerr base", Version = "v1" });
        });
    }
}