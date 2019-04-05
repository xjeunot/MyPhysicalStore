using XJeunot.PhysicalStoreApps.Services.Customer.API.Model;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.FlowValidation.Impl
{
    public interface ICustomerFlowValid
    {
        bool IsValidOperation(BaseValidatorType baseValidatorType, CustomerItem item1, CustomerItem item2);
    }
}
