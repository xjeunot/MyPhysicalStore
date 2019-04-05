using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl
{
    public interface ICheckOutFlowValid
    {
        bool IsValidOperation(BaseValidatorType baseValidatorType, CheckOutItem item1, CheckOutItem item2);
    }
}
