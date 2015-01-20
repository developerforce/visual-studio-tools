using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Salesforce.VisualStudio.Services.ConnectedService
{
    [Export(typeof(ConnectedServiceProvider))]
    [ExportMetadata(Constants.ProviderId, Constants.ProviderIdValue)]
    internal class SalesforceConnectedServiceProvider : ConnectedServiceProvider
    {
        public SalesforceConnectedServiceProvider()
        {
            this.Category = Resources.ConnectedServiceProvider_Category;
            this.CreatedBy = Resources.ConnectedServiceProvider_CreatedBy;
            this.Description = Resources.ConnectedServiceProvider_Description;
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + Assembly.GetAssembly(this.GetType()).ToString() + ";component/ConnectedService/Views/Resources/ProviderIcon.png"));
            this.MoreInfoUri = new Uri(Constants.MoreInfoLink);
            this.Name = Resources.ConnectedServiceProvider_Name;
            this.Version = typeof(SalesforceConnectedServiceProvider).Assembly.GetName().Version;
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderHost host)
        {
            ConnectedServiceConfigurator wizard = new SalesforceConnectedServiceWizard(host);

            return Task.FromResult(wizard);
        }
    }
}
