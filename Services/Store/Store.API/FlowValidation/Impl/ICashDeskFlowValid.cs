using XJeunot.PhysicalStoreApps.Services.Store.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.FlowValidation.Impl
{
    public interface ICashDeskFlowValid
    {
        bool IsValidOperation(BaseValidatorType baseValidatorType, CashDeskItem item1, CashDeskItem item2);
    }
}
