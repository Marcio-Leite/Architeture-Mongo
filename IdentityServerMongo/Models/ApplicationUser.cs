using System;
using System.Collections.Generic;

namespace IdentityServerMongo.Models
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public List<ApplicationRole> Roles { get; set; } 
        public string Password { get; set; }  
    }
}