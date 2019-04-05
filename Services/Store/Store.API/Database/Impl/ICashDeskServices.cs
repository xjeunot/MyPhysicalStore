using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XJeunot.PhysicalStoreApps.Services.Store.API.Database.Impl
{
    public interface ICashDeskServices
    {
        Task<IEnumerable<Model.CashDeskItem>> GetCashDeskList();

        Task<Model.CashDeskItem> GetCashDesk(string id);

        void AddCashDesk(Model.CashDeskItem model);

        Task<bool> UpdateCashDesk(string id, Model.CashDeskItem model);

        Task<DeleteResult> DeleteCashDesk(string Id);
    }
}
