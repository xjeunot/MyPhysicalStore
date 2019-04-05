using MongoDB.Bson.Serialization.Attributes;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Model
{
    public class CashDeskItem : BaseItem
    {
        public static string STATE_OPEN = "open";
        public static string STATE_LAST_CUSTOMER = "last_customer";
        public static string STATE_CLOSE = "close";

        public string CurrentState { get; set; }

        public string Name { get; set; }

        public string CashierName { get; set; }

        [BsonIgnore]
        public string SessionName => $"{Created.Ticks}_{Name}_{CashierName}";

        public CashDeskItem() : base()
        {
            CurrentState = STATE_CLOSE;
        }
    }
}
