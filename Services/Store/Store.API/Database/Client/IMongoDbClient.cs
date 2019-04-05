using MongoDB.Driver;
using System;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Database.Client
{
    public interface IMongoDbClient : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IMongoDatabase GetMongoDatabase();
    }
}
