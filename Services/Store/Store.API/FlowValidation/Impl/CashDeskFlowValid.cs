using System;
using System.Collections.Generic;
using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl
{
    public class CashDeskFlowValid : BaseFlowValid<CashDeskItem>, ICashDeskFlowValid
    {
        public static string BASE_STATE_ERROR = "error";

        protected override void InitListState()
        {
            // Default : Add StateError.
            AddStates(new BaseStateFlowValid(BASE_STATE_ERROR, false, false, false, new List<string>()));

            // Add State STATE_OPEN.
            AddStates(new BaseStateFlowValid(CashDeskItem.STATE_OPEN, false, true, false, new List<string>()
            {
                CashDeskItem.STATE_LAST_CUSTOMER
            }));

            // Add State STATE_LAST_CUSTOMER.
            AddStates(new BaseStateFlowValid(CashDeskItem.STATE_LAST_CUSTOMER, false, true, false, new List<string>()
            {
                CashDeskItem.STATE_CLOSE
            }));

            // Add State STATE_CLOSE.
            AddStates(new BaseStateFlowValid(CashDeskItem.STATE_CLOSE, true, true, true, new List<string>()
            {
                CashDeskItem.STATE_OPEN
            }));
        }

        protected override string GetStateName(CashDeskItem cashDeskItem)
        {
            // Default : Set to StateError.
            string strCurrentState = BASE_STATE_ERROR;

            // Evaluation STATE_OPEN.
            if (cashDeskItem.CurrentState == CashDeskItem.STATE_OPEN)
                strCurrentState = CashDeskItem.STATE_OPEN;

            // Evaluation STATE_LAST_CUSTOMER.
            if (cashDeskItem.CurrentState == CashDeskItem.STATE_LAST_CUSTOMER)
                strCurrentState = CashDeskItem.STATE_LAST_CUSTOMER;

            // Evaluation STATE_CLOSE.
            if (cashDeskItem.CurrentState == CashDeskItem.STATE_CLOSE)
                strCurrentState = CashDeskItem.STATE_CLOSE;

            // Return.
            return strCurrentState;
        }

        public bool IsValidOperation(BaseValidatorType baseValidatorType, CashDeskItem item1, CashDeskItem item2)
        {
            return base.IsValidOperationBase(baseValidatorType, item1, item2);
        }
    }
}
