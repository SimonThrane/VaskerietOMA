using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VaskerietOMA.Startup))]
namespace VaskerietOMA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
