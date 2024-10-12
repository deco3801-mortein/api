using Microsoft.OpenApi.Models;
using Mortein.Mqtt.Extensions;
using Mortein.Types;
using NodaTime.Serialization.SystemTextJson;
using System.Reflection;

namespace Mortein;

/// <summary>
/// Configure the application's startup.
/// </summary>
///
/// <param name="configuration"></param>
public class Startup(IConfiguration configuration)
{
    /// <summary>
    ///
    /// </summary>
    public IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Add services to the container.
    /// </summary>
    ///
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(NodaConverters.InstantConverter);
        });
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy.AllowAnyOrigin());
        });
        services.AddDbContext<DatabaseContext>();
        services.AddEndpointsApiExplorer();
        services.AddMqttClientHostedService();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("openapi", new OpenApiInfo()
            {
                Title = "Mortein API",
                Version = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version,
                Description = "RESTful API for the Mortein backend application",
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    /// <summary>
    /// Configure the HTTP request pipeline.
    /// </summary>
    ///
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseSwagger(c =>
            {
                c.RouteTemplate = "{documentName}.json";
            });
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Mortein API Docs";
            c.RoutePrefix = "docs";
            c.SwaggerEndpoint("/openapi.json", "Mortein API");
        });

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DatabaseContext>();
            context.Database.EnsureCreated();
        }

        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapGet("/", () => "Hello World!")
                .WithOpenApi(operation => new(operation)
                {
                    Summary = "Index",
                    Description = "Ping application."
                }
            );
        });
    }
}
