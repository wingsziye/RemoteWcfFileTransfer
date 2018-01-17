using Prism.Events;
using Remote.Infrastructure.PublicModels;

namespace RemoteClient.Beta.Events
{
    public class FileSendStartEvent : PubSubEvent<ProgressMessage>
    {

    }

    public class FileSendProgressChangedEvent : PubSubEvent<string>
    {
        
    }

    public class FileSendOverEvent : PubSubEvent<string>
    {
        
    }
}
