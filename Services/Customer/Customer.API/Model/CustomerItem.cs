using System;
using System.Linq;
using System.Collections.Generic;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Model
{
    public class CustomerItem : BaseItem
    {
        internal static readonly string STATE_NEW = "new";
        internal static readonly string STATE_KNOWN = "known";

        public string Name { get; set; }

        public int LoyaltyPoints { get; internal set; }

        // Only Set by Bus Event.
        public IList<CustomerCheckOut> LastCheckOut { get; internal set;}

        public CustomerItem() : base()
        {
            this.LoyaltyPoints = 0;
            this.LastCheckOut = new List<CustomerCheckOut>();
        }

        public void AddCheckOut(string _cashDeskSessionName, float _amountStore, DateTime _dateStore)
        {
            // Create CustomerCheckOut.
            CustomerCheckOut customerCheckOut = new CustomerCheckOut()
            {
                CashDeskSessionName = _cashDeskSessionName,
                AmountStore = _amountStore,
                DateStore = _dateStore
            };

            // We keep only the last 50 purchases.
            int countRecordDelete = this.LastCheckOut.Count - 50;
            if (countRecordDelete > 0)
            {
                for(int i = 0; i < countRecordDelete; i++)
                {
                    CustomerCheckOut remove = this.LastCheckOut
                        .OrderBy(x => x.DateStore)
                        .First();
                    if (remove != null) this.LastCheckOut.Remove(remove);
                }
            }

            // Add Element.
            this.LastCheckOut.Add(customerCheckOut);

            // Add LoyaltyPoints.
            int numberPointCheckOut = 1;
            if (customerCheckOut.AmountStore > 50) numberPointCheckOut += 2;
            if (customerCheckOut.AmountStore > 100) numberPointCheckOut += 3;
            if (customerCheckOut.AmountStore > 150) numberPointCheckOut += 4;
            if (customerCheckOut.AmountStore > 600) numberPointCheckOut += 10;
            this.LoyaltyPoints += numberPointCheckOut;
        }
    }
}
