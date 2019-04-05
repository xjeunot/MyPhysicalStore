using Microsoft.Azure.ServiceBus;
using System;

namespace XJeunot.PhysicalStoreApps.BuildingBlocks.EventBusAzure
{
    public interface IAzureServiceBusPersisterConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        ITopicClient CreateModel();

        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }
    }
}
