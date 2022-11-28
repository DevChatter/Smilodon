using Ardalis.Specification;

namespace Smilodon.Domain.Persistence;

public interface IRepository<T> : IRepositoryBase<T> where T : class
{
}
