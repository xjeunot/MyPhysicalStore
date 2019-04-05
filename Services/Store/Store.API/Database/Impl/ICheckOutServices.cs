using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl
{
    public interface ICheckOutServices
    {
        Task<IEnumerable<Model.CheckOutItem>> GetCheckOutList();

        Task<Model.CheckOutItem> GetCheckOut(string id);

        void AddCheckOut(Model.CheckOutItem model);

        Task<bool> UpdateCheckOut(string id, Model.CheckOutItem model);

        Task<DeleteResult> DeleteCheckOut(string Id);
    }
}
