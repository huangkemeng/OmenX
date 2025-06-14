using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OmenX.Contracts;

namespace OmenX.Extensions;

public static class OmenXServiceExtensions
{
    public static IServiceCollection AddOmenX(this IServiceCollection services, params Assembly[] assemblies)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        if (!assemblies.Contains(assembly))
        {
            ScanCheckPoint(services, assembly);
        }
        else
        {
            foreach (var assemblyItem in assemblies)
            {
                ScanCheckPoint(services, assembly);
            }
        }

        return services;
    }

    private static void ScanCheckPoint(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.GetInterface(typeof(IOmenXCheckPoint).FullName) != null)
            {
                services.AddScoped(typeof(IOmenXCheckPoint), type);
            }
        }
    }
}