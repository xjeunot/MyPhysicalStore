using System.Collections.Generic;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation
{
    public class BaseStateFlowValid
    {
        public string CurrentState { get; internal set; }

        public bool IsStatePermitCreate { get; internal set; }

        public bool IsStatePermitEdit { get; internal set; }

        public bool IsStatePermitDelete { get; internal set; }

        public IEnumerable<string> StatesNext { get; internal set; }

        public BaseStateFlowValid(string _currentState,
            bool _isStatePermitCreate, bool _isStatePermitEdit, bool _isStatePermitDelete,
            IEnumerable<string> _statesNext)
        {
            this.CurrentState = _currentState;
            this.IsStatePermitCreate = _isStatePermitCreate;
            this.IsStatePermitEdit = _isStatePermitEdit;
            this.IsStatePermitDelete = _isStatePermitDelete;
            this.StatesNext = _statesNext;
        }
    }
}
