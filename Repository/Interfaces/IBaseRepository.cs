using Repository.Provider;

namespace Repository.Interfaces
{
    public interface IBaseRepository
    {
        IUnitOfWork UnitOfWork { get; set; }
    }
}