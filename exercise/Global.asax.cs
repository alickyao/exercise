using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Security.Principal;
using cyclonestyle.BLL;

namespace cyclonestyle
{
    public class MvcApplication : HttpApplication
    {

        public MvcApplication()
        {
            AuthorizeRequest += new EventHandler(MvcApplication_AuthorizeRequest);
        }

        void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            IIdentity id = Context.User.Identity;
            if (id.IsAuthenticated)
            {
                //var roles = new UsersService().GetRoles(id.Name);
                try
                {
                    SysManagerService sms = new SysManagerService();
                    sms.GetSysUser(id.Name);
                    var roles = sms.SysUserInfo.SysRole.RoleName.Split(',');
                    Context.User = new GenericPrincipal(id, roles);
                }
                catch (Exception error)
                {

                }
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}