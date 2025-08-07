using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MyApp.Application.Extensions;
using MyApp.ConsoleApp.Services;
using MyApp.Infrastructure.Extensions;
//dotnet add package Serilog
//dotnet add package Serilog.AspNetCore
//dotnet add package Serilog.Sinks.Console
//dotnet add package Serilog.Sinks.File
//dotnet add package Serilog.Settings.Configuration

using Serilog;

namespace MyApp.ConsoleApp;
public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day, encoding: Encoding.UTF8)
            .CreateLogger();

        try
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var app = scope.ServiceProvider.GetRequiredService<IAppRunner>();
                app.Run();
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application caused fatal error");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console();
            })
            .ConfigureServices((context, services) =>
            {
                // Register DI for projects
                services.AddApplication();
                services.AddInfrastructure(context.Configuration);
                services.AddPersistence();

                // Register app entry point
                services.AddTransient<IAppRunner, AppRunner>();
            });
}