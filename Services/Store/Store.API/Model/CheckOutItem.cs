namespace XJeunot.PhysicalStoreApps.Services.Store.API.Model
{
    public class CheckOutItem : BaseItem
    {
        public static string STATE_PAID = "paid";
        public static string STATE_CANCELED = "canceled";
        public static string STATE_CLOSED = "closed";

        public string CurrentState { get; set; }

        public string CustomerId { get; set; }

        public float AmountStore { get; set; }

        public string CashDeskSessionName { get; set; }

        public CheckOutItem() : base()
        {
            CurrentState = STATE_PAID;
        }
    }
}
