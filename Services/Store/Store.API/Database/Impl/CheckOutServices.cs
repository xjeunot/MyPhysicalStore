using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using XJeunot.PhysicalStoreApps.Services.Store.API.Database.Client;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl
{
    public class CheckOutServices : ICheckOutServices
    {
        private readonly IMongoDbClient _client = null;

        public CheckOutServices(IMongoDbClient client)
        {
            _client = client;
        }

        public IMongoCollection<Model.CheckOutItem> Collection()
        {
            if (!_client.IsConnected)
                _client.TryConnect();
            IMongoDatabase iMongoDatabase = _client.GetMongoDatabase();
            return iMongoDatabase.GetCollection<Model.CheckOutItem>("checkout");
        }

        public void AddCheckOut(CheckOutItem model)
        {
            Collection().InsertOne(model);
        }

        public async Task<DeleteResult> DeleteCheckOut(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            var filtre = Builders<Model.CheckOutItem>.Filter.Eq("InternalId", objectId);
            return await Collection().DeleteOneAsync(filtre);
        }

        public async Task<CheckOutItem> GetCheckOut(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            var filter = Builders<Model.CheckOutItem>.Filter.Eq("InternalId", objectId);
            return await Collection().Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CheckOutItem>> GetCheckOutList()
        {
            return await Collection().Find(x => true).ToListAsync();
        }

        public async Task<bool> UpdateCheckOut(string id, CheckOutItem model)
        {
            if (!ObjectId.TryParse(model.Id, out ObjectId objectId)) return false;

            var filter = Builders<Model.CheckOutItem>.Filter.Eq("InternalId", objectId);
            var client = Collection().Find(filter).FirstOrDefaultAsync();
            if (client.Result == null)
                return false;

            var objectUpdate = Builders<Model.CheckOutItem>.Update
                .Set(x => x.CurrentState, model.CurrentState)
                .Set(x => x.CashDeskSessionName, model.CashDeskSessionName)
                .Set(x => x.Update, DateTime.Now);

            await Collection().UpdateOneAsync(filter, objectUpdate);
            return true;
        }
    }
}
