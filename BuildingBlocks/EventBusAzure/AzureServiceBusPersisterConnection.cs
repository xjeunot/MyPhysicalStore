using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;

namespace XJeunot.PhysicalStoreApps.BuildingBlocks.EventBusAzure
{
    public class AzureServiceBusPersisterConnection : IAzureServiceBusPersisterConnection
    {
        private readonly ILogger<AzureServiceBusPersisterConnection> _logger;
        private ITopicClient _topicClient;

        bool _disposed;

        object sync_root = new object();

        public AzureServiceBusPersisterConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder,
            ILogger<AzureServiceBusPersisterConnection> logger)
        {
            ServiceBusConnectionStringBuilder = serviceBusConnectionStringBuilder;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsConnected
        {
            get
            {
                return _topicClient != null && _topicClient.IsClosedOrClosing != true && !_disposed;
            }
        }

        public ITopicClient CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No AzureBus connections are available to perform this action");
            }
            return _topicClient;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _topicClient.CloseAsync();
                _topicClient = null;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("AzureBus Client is trying to connect");

            lock (sync_root)
            {
                _topicClient = new TopicClient(ServiceBusConnectionStringBuilder);
                if (IsConnected)
                {
                    _logger.LogInformation($"AzureBus persistent connection acquired a connection");
                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: AzureBus connections could not be created and opened");

                    return false;
                }
            }
        }

        public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }
    }
}
