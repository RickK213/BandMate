using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BandMate.Startup))]
namespace BandMate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
