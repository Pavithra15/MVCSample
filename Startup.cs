using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EJ2MVC.Startup))]
namespace EJ2MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
