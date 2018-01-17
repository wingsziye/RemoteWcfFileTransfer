using Remote.Infrastructure.Interfaces;

namespace Remote.Infrastructure.PublicEvents
{
    public class ClientTokenFirstUpdateEvent
    {
        public IClientToken User { get; set; }
    }
}
