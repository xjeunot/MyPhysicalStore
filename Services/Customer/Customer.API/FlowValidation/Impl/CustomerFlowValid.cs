using System;
using System.Collections.Generic;
using XJeunot.PhysicalStoreApps.Services.Customer.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation.Impl
{
    public class CustomerFlowValid : BaseFlowValid<CustomerItem>, ICustomerFlowValid
    {
        public static string BASE_STATE_ERROR = "error";

        protected override void InitListState()
        {
            // Default : Add StateError.
            AddStates(new BaseStateFlowValid(BASE_STATE_ERROR, false, false, false, new List<string>()));

            // Add State STATE_NEW.
            AddStates(new BaseStateFlowValid(CustomerItem.STATE_NEW, true, true, true, new List<string>()
            {
                CustomerItem.STATE_NEW,
                CustomerItem.STATE_KNOWN
            }));

            // Add State STATE_KNOWN.
            AddStates(new BaseStateFlowValid(CustomerItem.STATE_KNOWN, false, true, false, new List<string>()
            {
                CustomerItem.STATE_KNOWN
            }));
        }

        protected override string GetStateName(CustomerItem customerItem)
        {
            // Default : Set to StateError.
            string strCurrentState = BASE_STATE_ERROR;

            // Evaluation STATE_NEW.
            if ((customerItem.LastCheckOut.Count == 0) &&
                (customerItem.LoyaltyPoints == 0))
                strCurrentState = CustomerItem.STATE_NEW;

            // Evaluation STATE_KNOWN.
            if ((customerItem.LastCheckOut.Count != 0) &&
                (customerItem.LoyaltyPoints != 0))
                strCurrentState = CustomerItem.STATE_KNOWN;

            // Return.
            return strCurrentState;
        }

        public bool IsValidOperation(BaseValidatorType baseValidatorType, CustomerItem item1, CustomerItem item2)
        {
            return base.IsValidOperationBase(baseValidatorType, item1, item2);
        }
    }
}
