using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XJeunot.PhysicalStoreApps.Services.Customer.API.Database.Impl
{
    public interface ICustomerServices
    {
        Task<IEnumerable<Model.CustomerItem>> GetCustomerList();

        Task<Model.CustomerItem> GetCustomer(string id);

        void AddCustomer(Model.CustomerItem model);

        Task<bool> UpdateCustomer(string id, Model.CustomerItem model);

        Task<bool> UpdateCustomerLastCheckOut(string id, Model.CustomerItem model);

        Task<DeleteResult> DeleteCustomer(string Id);
    }
}
