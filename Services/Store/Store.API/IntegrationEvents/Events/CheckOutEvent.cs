using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Events;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.IntegrationEvents.Events
{
    public class CheckOutEvent : IntegrationEvent
    {
        public string CashDeskSessionName { get; set; }

        public string CustomerId { get; set; }

        public float AmountStore { get; set; }

        public static CheckOutEvent FromItem(CheckOutItem _checkOutItem)
        {
            return new CheckOutEvent()
            {
                CashDeskSessionName = _checkOutItem.CashDeskSessionName,
                CustomerId = _checkOutItem.CustomerId,
                AmountStore = _checkOutItem.AmountStore
            };
        }
    }
}
