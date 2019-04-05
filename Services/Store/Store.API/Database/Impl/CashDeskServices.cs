using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using XJeunot.PhysicalStoreApps.Services.Store.API.Database.Client;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl
{
    public class CashDeskServices : ICashDeskServices
    {
        private readonly IMongoDbClient _client = null;

        public CashDeskServices(IMongoDbClient client)
        {
            _client = client;
        }

        public IMongoCollection<Model.CashDeskItem> Collection()
        {
            if (!_client.IsConnected)
                _client.TryConnect();
            IMongoDatabase iMongoDatabase = _client.GetMongoDatabase();
            return iMongoDatabase.GetCollection<Model.CashDeskItem>("cashdesk");
        }

        public void AddCashDesk(CashDeskItem model)
        {
            Collection().InsertOne(model);
        }

        public async Task<DeleteResult> DeleteCashDesk(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            var filtre = Builders<Model.CashDeskItem>.Filter.Eq("InternalId", objectId);
            return await Collection().DeleteOneAsync(filtre);
        }

        public async Task<CashDeskItem> GetCashDesk(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId)) return null;

            var filter = Builders<Model.CashDeskItem>.Filter.Eq("InternalId", objectId);
            return await Collection().Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CashDeskItem>> GetCashDeskList()
        {
            return await Collection().Find(x => true).ToListAsync();
        }

        public async Task<bool> UpdateCashDesk(string id, CashDeskItem model)
        {
            if (!ObjectId.TryParse(model.Id, out ObjectId objectId)) return false;

            var filter = Builders<Model.CashDeskItem>.Filter.Eq("InternalId", objectId);
            var client = Collection().Find(filter).FirstOrDefaultAsync();
            if (client.Result == null)
                return false;

            var objectUpdate = Builders<Model.CashDeskItem>.Update
                .Set(x => x.CurrentState, model.CurrentState)
                .Set(x => x.CashierName, model.CashierName)
                .Set(x => x.Update, DateTime.Now);

            await Collection().UpdateOneAsync(filter, objectUpdate);
            return true;
        }
    }
}
