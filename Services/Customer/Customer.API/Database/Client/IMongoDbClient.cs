using MongoDB.Driver;
using System;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Client
{
    public interface IMongoDbClient : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IMongoDatabase GetMongoDatabase();
    }
}
