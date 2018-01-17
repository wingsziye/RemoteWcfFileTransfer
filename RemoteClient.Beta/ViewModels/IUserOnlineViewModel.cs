using System.Collections.ObjectModel;
using Remote.Infrastructure.DataContracts;
using RemoteClient.Inferstructure;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.ViewModels
{
    public interface IUserOnlineViewModel:IViewModel
    {
        RemoteOnlineClientProxy ClientProxy { get; set; }

        ClientToken SelfToken { get; set; }

        ObservableCollection<ClientToken> OnlineClients { get; set; }
    }
}
