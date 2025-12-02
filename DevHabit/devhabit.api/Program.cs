using devhabit.api.Database;
using devhabit.api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("database"),
            npgsqloptions => npgsqloptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Application))
           .UseSnakeCaseNamingConvention();
});

builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => 
                {
                    resource.AddService(builder.Environment.ApplicationName);
                    
                })
                .WithTracing(tracing =>


                {
                    tracing.AddHttpClientInstrumentation()
                           .AddAspNetCoreInstrumentation()
                           ;
                    tracing.AddNpgsql();
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddHttpClientInstrumentation()
                           .AddAspNetCoreInstrumentation()
                           .AddRuntimeInstrumentation();
                })
                .UseOtlpExporter();

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
    options.ParseStateValues = true;
});
                

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();


app.MapControllers();

await app.RunAsync();
