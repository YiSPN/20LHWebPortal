using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_20LHWebPortal.Startup))]
namespace _20LHWebPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
