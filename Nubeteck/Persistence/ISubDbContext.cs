using Microsoft.EntityFrameworkCore;

namespace Nubeteck.Extensions.Persistence;

public interface ISubDbContext
{
    IEnumerable<Type> GetEntities();
    void ConfigureModel(ModelBuilder modelBuilder);
}
