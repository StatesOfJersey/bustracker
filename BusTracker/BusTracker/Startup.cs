using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BusTracker.Startup))]
namespace BusTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
