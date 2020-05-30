using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoCoreDbRepository.Interfaces;
using MongoDB.Driver;

namespace IdentityServerMongo.Repository
{
    public class UserMongoContext: IMongoContext
    { 
        private IMongoDatabase _database { get; set; }
        public IClientSessionHandle session { get; set; }
        public MongoClient mongoClient { get; set; }
        private readonly List<Func<Task>> _commands;
        private readonly IConfiguration _configuration;

        public UserMongoContext(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // Os comandos serão armazenados e processados em SaveChanges
            _commands = new List<Func<Task>>();
        }
        
        private void ConfigureMongo()
        {
            if (mongoClient != null) return;
            mongoClient = new MongoClient(_configuration["MongoSettings:Connection"]);
            _database = mongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]);
        }
        
        public async Task<int> SaveChanges()
        {
            ConfigureMongo();

            using (session = await mongoClient.StartSessionAsync())
            {
                session.StartTransaction();

                var commandTasks = _commands.Select(c => c());
                await Task.WhenAll(commandTasks);

                await session.CommitTransactionAsync();
            }

            return _commands.Count;
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }
        
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            ConfigureMongo();
            return _database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}