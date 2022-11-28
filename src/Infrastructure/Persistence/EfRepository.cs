using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smilodon.Domain.Persistence;

namespace Smilodon.Infrastructure.Persistence;

public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public EfRepository(SmilodonDbContext dbContext) : base(dbContext)
    {
    }
}
