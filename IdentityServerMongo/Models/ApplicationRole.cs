using System;


namespace IdentityServerMongo.Models
{
    //adicionar campos customizados
    public class ApplicationRole 
    {
        public ApplicationRole(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}