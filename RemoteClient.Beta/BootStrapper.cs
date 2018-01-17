using Prism.Modularity;
using Prism.Unity;
using RemoteClient.Beta.Modules;
using System.Windows;
using Microsoft.Practices.Unity;

namespace RemoteClient.Beta
{
    public class BootStrapper : UnityBootstrapper
    {
        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            Container.RegisterType<Shell>();
            return Container.Resolve<Shell>();
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            ModuleCatalog catalog = new ModuleCatalog();
            return catalog;
        }

        protected override void ConfigureModuleCatalog()
        {
            var cataLog = ModuleCatalog as ModuleCatalog;
            cataLog.AddModule(typeof(CoreModule));
            cataLog.AddModule(typeof(ProgressModule), InitializationMode.OnDemand);
            base.ConfigureModuleCatalog();
        }
    }
}
