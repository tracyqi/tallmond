using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenOrders.Startup))]
namespace OpenOrders
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
