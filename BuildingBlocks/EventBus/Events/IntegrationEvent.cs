using System;
using Newtonsoft.Json;

namespace XJeunot.PhysicalStoreApps.BuildingBlocks.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            _Id = Guid.NewGuid();
            _CreationDate = DateTime.Now;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            _Id = id;
            _CreationDate = createDate;
        }

        [JsonProperty]
        public Guid _Id { get; private set; }

        [JsonProperty]
        public DateTime _CreationDate { get; private set; }
    }
}
