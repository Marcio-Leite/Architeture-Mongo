using IdentityServerMongo.Models;
using MongoCoreDbRepository.Interfaces;
using MongoCoreDbRepository.Repository;

namespace IdentityServerMongo.Repository
{
    public class UserRepository: BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(IMongoContext context) : base(context)
        {
        }
    }
}