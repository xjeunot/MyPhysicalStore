using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.IO;
using System.Security.Authentication;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Client
{
    public class MongoDbClient : IMongoDbClient
    {
        private readonly IOptions<MongoDbConfig> _settings;
        private readonly ILogger<MongoDbClient> _logger;

        private IMongoClient _client;
        bool _dispose;

        public MongoDbClient(IOptions<MongoDbConfig> settings, ILogger<MongoDbClient> logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsConnected
        {
            get
            {
                return _client != null && !_dispose;
            }
        }

        public void Dispose()
        {
            if (_dispose) return;

            _dispose = true;

            try
            {
                /*
                 * MongoDB manage connection close :
                 * - https://stackoverflow.com/questions/9172360/when-should-i-be-opening-and-closing-mongodb-connections
                 */
                _client = null;
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public IMongoDatabase GetMongoDatabase()
        {
            if (!IsConnected)
            {
                _logger.LogCritical("Action not possible: No connection to MongoDB");
                return null;
            }
            return _client.GetDatabase(_settings.Value.Datebase);
        }

        public bool TryConnect()
        {
            _logger.LogInformation("MongoDB Client is trying to connect");
            try
            {
                if (_settings.Value.IsAzure)
                {
                    MongoClientSettings settings = MongoClientSettings.FromUrl(
                        new MongoUrl(_settings.Value.ConnectionString));
                    settings.SslSettings =
                        new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                    _client = new MongoClient(settings);
                }
                else
                {
                    MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(_settings.Value.ConnectionString));
                    if ((_settings.Value.User != "") &&
                        (_settings.Value.Password != ""))
                    {
                        MongoCredential clsMongoCredential = MongoCredential.CreateCredential(
                            "admin",
                            _settings.Value.User,
                            _settings.Value.Password);
                        settings.Credential = clsMongoCredential;
                    }
                    _client = new MongoClient(settings);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("FATAL ERROR: MongoDB connections could not be created and opened");
                _logger.LogCritical(ex.Message);
                return false;
            }
        }
    }
}
