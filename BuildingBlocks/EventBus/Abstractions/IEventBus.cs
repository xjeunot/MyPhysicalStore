using XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Events;

namespace XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Abstractions
{
    public interface IEventBus
    {
        void ActivateConsumerChannel();

        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}
