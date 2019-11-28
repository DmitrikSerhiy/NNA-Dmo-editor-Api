using DAL.Repositories.Interfaces;

namespace DAL {
    public interface IUnitOfWork {
        ICustomerRepository Customers { get; }
        int SaveChanges();
    }
}
