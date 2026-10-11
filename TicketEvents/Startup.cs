using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketEvents.Startup))]
namespace TicketEvents
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}