using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Model
{
    public abstract class BaseItem
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId InternalId { get; set; }

        [BsonIgnore]
        public string Id
        {
            get
            {
                return InternalId != ObjectId.Empty ? InternalId.ToString() : string.Empty;
            }
            set
            {
                ObjectId objectId = ObjectId.Empty;
                if ((value != null) &&
                    (value.Trim() != string.Empty) &&
                    (ObjectId.TryParse(value, out objectId)))
                    InternalId = objectId;
            }
        }

        public DateTime Created { get; set; }

        public DateTime Update { get; set; }

        public BaseItem()
        {
            Created = DateTime.Now;
            Update = DateTime.Now;
        }
    }
}
