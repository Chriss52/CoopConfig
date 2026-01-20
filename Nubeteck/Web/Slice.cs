using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace Nubeteck.Extensions.Web;

public interface ISlice
{
    void RegisterRoutes(IEndpointRouteBuilder app);
    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}

public static class SliceExtensions
{
    public static IServiceCollection RegisterSlices(this IServiceCollection services, IConfiguration configuration, Assembly currentAssembly)
    {
        var slices = currentAssembly.DefinedTypes.Where(t =>
           t.ImplementedInterfaces.Contains(typeof(ISlice))
           );

        foreach (var slice in slices)
        {
            services.AddSingleton(typeof(ISlice), slice);
            var serviceProvider = services.BuildServiceProvider();
            var sliceInstance = serviceProvider.GetRequiredService<ISlice>();
            sliceInstance.RegisterServices(services, configuration);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapSliceEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        foreach (ISlice slice in endpointRouteBuilder.ServiceProvider.GetServices<ISlice>())
        {
            slice.RegisterRoutes(endpointRouteBuilder);
        }

        return endpointRouteBuilder;
    }
}
