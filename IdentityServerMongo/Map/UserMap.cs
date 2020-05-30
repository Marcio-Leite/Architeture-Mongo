using IdentityServerMongo.Models;
using MongoDB.Bson.Serialization;

namespace IdentityServerMongo.Map
{
    public class UserMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<ApplicationUser>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
                map.MapMember(x => x.Name).SetIsRequired(true);
                map.MapMember(x => x.LastName);
                map.MapMember(x => x.Email).SetIsRequired(true);
                map.MapMember(x => x.UserName).SetIsRequired(true);
                map.MapMember(x => x.Password).SetIsRequired(true);
            });
        }
    }
}