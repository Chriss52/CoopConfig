using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Nubeteck.Extensions.Persistence;

public static class DataContextExtension
{
    public static void RegisterSubModels(this ModelBuilder modelBuilder)
    {
        var subContextTypes = Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(t => typeof(ISubDbContext).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        var subContexts = subContextTypes
            .Select(Activator.CreateInstance)
            .Cast<ISubDbContext>();

        foreach (var subContext in subContexts)
        {
            foreach (var entityType in subContext.GetEntities())
            {
                modelBuilder.Entity(entityType);
            }

            subContext.ConfigureModel(modelBuilder);
        }
    }
}
