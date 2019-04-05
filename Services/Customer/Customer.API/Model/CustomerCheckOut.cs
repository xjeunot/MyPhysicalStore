using System;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Model
{
    public class CustomerCheckOut
    {
        public string CashDeskSessionName { get; set; }

        public float AmountStore { get; set; }

        public DateTime DateStore { get; set; }
    }
}
