using Remote.Infrastructure.Interfaces;

namespace Remote.Infrastructure.PublicEvents
{
    public class ClientTokenOnlineStateChangedEvent
    {
        public IClientToken User { get; set; }
    }
}
