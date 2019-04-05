using System.Threading.Tasks;

namespace XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
