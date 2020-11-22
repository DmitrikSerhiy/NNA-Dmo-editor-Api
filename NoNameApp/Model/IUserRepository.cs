using System;
using System.Threading.Tasks;
using Model.Entities;

namespace Model {
    public interface IUserRepository {
        Task<NnaUser> WithId(Guid id);
        Task<NnaUser> FirstUser();
    }
}
