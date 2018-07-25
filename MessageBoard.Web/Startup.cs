using Microsoft.Owin;

[assembly: OwinStartup(typeof(MessageBoard.Web.Startup))]
namespace MessageBoard.Web
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);

            // Configure SignalR
            app.MapSignalR();
        }
    }
}