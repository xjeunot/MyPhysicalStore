using System;
using System.Collections.Generic;
using System.Linq;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation
{
    public abstract class BaseFlowValid<T>
    {
        private readonly IList<BaseStateFlowValid> ListFlows;

        protected abstract void InitListState();

        protected abstract string GetStateName(T t_from);

        public BaseFlowValid()
        {
            // Init the list of BaseObjectState.
            this.ListFlows = new List<BaseStateFlowValid>();
            InitListState();
        }

        protected bool IsValidOperationBase(BaseValidatorType type, T t_from, T t_to)
        {
            /*
             * Create.
             */
            if (type == BaseValidatorType.Create)
            {
                string strStateName = GetStateName(t_from);
                BaseStateFlowValid flow = this.ListFlows
                    .Where(x => x.CurrentState == strStateName)
                    .FirstOrDefault();
                return flow.IsStatePermitCreate;
            }

            /*
             * Edit.
             */
            if (type == BaseValidatorType.Update)
            {
                string strStateName = GetStateName(t_from);
                BaseStateFlowValid flow = this.ListFlows
                    .Where(x => x.CurrentState == strStateName)
                    .FirstOrDefault();
                return flow.IsStatePermitEdit && flow.StatesNext.Contains(GetStateName(t_to));
            }

            /*
             * Delete.
             */
            if (type == BaseValidatorType.Delete)
            {
                string strStateName = GetStateName(t_from);
                BaseStateFlowValid flow = this.ListFlows
                    .Where(x => x.CurrentState == strStateName)
                    .FirstOrDefault();
                return flow.IsStatePermitDelete;
            }

            /*
             * Default.
             */
            return false;
        }

        protected void AddStates(BaseStateFlowValid _flow)
        {
            // Check that the state does not exist.
            int countExist = this.ListFlows
                .Count(x => x.CurrentState == _flow.CurrentState);
            if (countExist != 0)
            {
                throw new System.Exception("Internal Use Error : The State " +
                    _flow.CurrentState +
                    " is present into the list");
            }
            // Add.
            this.ListFlows.Add(_flow);
        }
    }

    public enum BaseValidatorType
    {
        Create,
        Update,
        Delete
    }
}
