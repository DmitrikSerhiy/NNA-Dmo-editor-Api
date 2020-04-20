using System;
using System.Threading.Tasks;
using Model.Entities;

namespace Model {
    public interface IUserRepository {
        Task<NoNameUser> WithId(Guid id);
        Task<NoNameUser> FirstUser();
    }
}
