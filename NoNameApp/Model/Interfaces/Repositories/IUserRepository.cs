using System;
using System.Threading.Tasks;
using Model.Entities;

namespace Model.Interfaces.Repositories {
    public interface IUserRepository {
        Task<NnaUser> WithId(Guid id);
        Task<NnaUser> FirstUser();
    }
}
