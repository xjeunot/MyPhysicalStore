using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Client;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Impl
{
    public class CustomerServices : ICustomerServices
    {
        private readonly IMongoDbClient _client = null;

        public CustomerServices(IMongoDbClient client)
        {
            _client = client;
        }

        public IMongoCollection<Model.CustomerItem> Collection()
        {
            if (!_client.IsConnected)
                _client.TryConnect();
            IMongoDatabase iMongoDatabase = _client.GetMongoDatabase();
            return iMongoDatabase.GetCollection<Model.CustomerItem>("customer");
        }

        public void AddCustomer(CustomerItem model)
        {
            Collection().InsertOne(model);
        }

        public async Task<DeleteResult> DeleteCustomer(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            var filtre = Builders<Model.CustomerItem>.Filter.Eq("InternalId", objectId);
            return await Collection().DeleteOneAsync(filtre);
        }

        public async Task<CustomerItem> GetCustomer(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            var filter = Builders<Model.CustomerItem>.Filter.Eq("InternalId", objectId);
            return await Collection().Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CustomerItem>> GetCustomerList()
        {
            return await Collection().Find(x => true).ToListAsync();
        }

        public async Task<bool> UpdateCustomer(string id, CustomerItem model)
        {
            if (!ObjectId.TryParse(model.Id, out ObjectId objectId)) return false;

            var filter = Builders<Model.CustomerItem>.Filter.Eq("InternalId", objectId);
            var client = Collection().Find(filter).FirstOrDefaultAsync();
            if (client.Result == null)
                return false;

            var objectUpdate = Builders<Model.CustomerItem>.Update
                .Set(x => x.Name, model.Name)
                .Set(x => x.Update, DateTime.Now);

            await Collection().UpdateOneAsync(filter, objectUpdate);
            return true;
        }

        public async Task<bool> UpdateCustomerLastCheckOut(string id, CustomerItem model)
        {
            if (!ObjectId.TryParse(model.Id, out ObjectId objectId)) return false;

            var filter = Builders<Model.CustomerItem>.Filter.Eq("InternalId", objectId);
            var client = Collection().Find(filter).FirstOrDefaultAsync();
            if (client.Result == null)
                return false;

            var objectUpdate = Builders<Model.CustomerItem>.Update
                .Set(x => x.LoyaltyPoints, model.LoyaltyPoints)
                .Set(x => x.LastCheckOut, model.LastCheckOut)
                .Set(x => x.Update, DateTime.Now);

            await Collection().UpdateOneAsync(filter, objectUpdate);
            return true;
        }
    }
}
