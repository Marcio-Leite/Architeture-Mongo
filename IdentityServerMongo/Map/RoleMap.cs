using IdentityServerMongo.Models;
using MongoDB.Bson.Serialization;

namespace IdentityServerMongo.Map
{
    public class RoleMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<ApplicationRole>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
                map.MapMember(x => x.Name).SetIsRequired(true);
            });
        }
    }
}