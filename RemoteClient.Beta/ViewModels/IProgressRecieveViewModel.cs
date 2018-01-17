using Remote.Infrastructure.DataContracts;
using Wpf.Infrastructure.MVVM;

namespace RemoteClient.Beta.ViewModels
{
    public interface IProgressReceiveViewModel:IViewModel
    {
        ClientToken SelfToken { get; set; }
    }
}
