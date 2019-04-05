using System.Threading.Tasks;
using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Impl;
using XJeunot.PhysicalStoreApps.Services.Customer.API.IntegrationEvents.Events;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.IntegrationEvents.EventHandling
{
    public class CheckOutEventHandler : IIntegrationEventHandler<CheckOutEvent>
    {
        private readonly ICustomerServices _iCustomerServices;

        public CheckOutEventHandler(ICustomerServices iCustomerServices)
        {
            _iCustomerServices = iCustomerServices;
        }

        public async Task Handle(CheckOutEvent command)
        {
            // Search Customer.
            CustomerItem customerItem = _iCustomerServices.GetCustomer(command.CustomerId).Result;
            if (customerItem == null) return;

            // Add CheckOut.
            customerItem.AddCheckOut(command.CashDeskSessionName, command.AmountStore, command._CreationDate);

            // Update Db.
            await _iCustomerServices.UpdateCustomerLastCheckOut(customerItem.Id, customerItem);
        }
    }
}
