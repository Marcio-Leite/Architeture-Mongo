using IdentityServerMongo.Models;
using MongoCoreDbRepository.Interfaces;
using MongoCoreDbRepository.Repository;

namespace IdentityServerMongo.Repository
{
    public class RoleRepository: BaseRepository<ApplicationRole>, IRoleRepository
    {
        public RoleRepository(IMongoContext context) : base(context)
        {
        }
    }
}