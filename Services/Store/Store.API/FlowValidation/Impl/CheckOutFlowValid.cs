using System.Collections.Generic;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl
{
    public class CheckOutFlowValid : BaseFlowValid<CheckOutItem>, ICheckOutFlowValid
    {
        public static string BASE_STATE_ERROR = "error";

        protected override void InitListState()
        {
            // Default : Add StateError.
            AddStates(new BaseStateFlowValid(BASE_STATE_ERROR, false, false, false, new List<string>()));

            // Add State STATE_PAID.
            AddStates(new BaseStateFlowValid(CheckOutItem.STATE_PAID, true, true, false, new List<string>()
            {
                CheckOutItem.STATE_PAID,
                CheckOutItem.STATE_CANCELED,
                CheckOutItem.STATE_CLOSED

            }));

            // Add State STATE_CANCELED.
            AddStates(new BaseStateFlowValid(CheckOutItem.STATE_CANCELED, false, false, true, new List<string>()
            {
            }));

            // Add State STATE_CLOSED.
            AddStates(new BaseStateFlowValid(CheckOutItem.STATE_CLOSED, false, false, false, new List<string>()
            {
            }));
        }

        protected override string GetStateName(CheckOutItem checkOutItem)
        {
            // Default : Set to StateError.
            string strCurrentState = BASE_STATE_ERROR;

            // Evaluation STATE_PAID.
            if (checkOutItem.CurrentState == CheckOutItem.STATE_PAID)
                strCurrentState = CheckOutItem.STATE_PAID;

            // Evaluation STATE_CANCELED.
            if (checkOutItem.CurrentState == CheckOutItem.STATE_CANCELED)
                strCurrentState = CheckOutItem.STATE_CANCELED;

            // Evaluation STATE_CLOSED.
            if (checkOutItem.CurrentState == CheckOutItem.STATE_CLOSED)
                strCurrentState = CheckOutItem.STATE_CLOSED;

            // Return.
            return strCurrentState;
        }

        public bool IsValidOperation(BaseValidatorType baseValidatorType, CheckOutItem item1, CheckOutItem item2)
        {
            return base.IsValidOperationBase(baseValidatorType, item1, item2);
        }
    }
}
