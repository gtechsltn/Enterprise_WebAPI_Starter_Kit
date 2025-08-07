using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MyApp.Application.Services;
using MyApp.Application.Services.Interfaces;


namespace MyApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICatalogService, CatalogService>();


        var executingAssembly = Assembly.GetExecutingAssembly();
        Debug.WriteLine(executingAssembly.FullName);

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Debug.WriteLine(asm.FullName);
        }

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}