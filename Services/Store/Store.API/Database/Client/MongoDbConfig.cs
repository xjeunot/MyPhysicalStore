namespace XJeunot.PhysicalStoreApps.Services.Store.API.Database.Client
{
    public class MongoDbConfig
    {
        
        public string ConnectionString { get; set; }

        public string Datebase { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public bool IsAzure { get; set; }
    }
}
