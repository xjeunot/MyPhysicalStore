using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Events;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.IntegrationEvents.Events
{
    public class CheckOutEvent : IntegrationEvent
    {
        public string CashDeskSessionName { get; set; }

        public string CustomerId { get; set; }

        public float AmountStore { get; set; }
    }
}
