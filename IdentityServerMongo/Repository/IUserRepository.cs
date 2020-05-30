using IdentityServerMongo.Models;
using MongoCoreDbRepository.Interfaces;

namespace IdentityServerMongo.Repository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        
    }
}