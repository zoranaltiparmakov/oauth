using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System.Linq;
using TODO_WebAPI.App_Start;
using TODO_WebAPI.OAuth;
using Microsoft.Owin.Security;
using TODO_WebAPI;

[assembly: OwinStartup(typeof(TODO_WebAPI.Startup))]

namespace TODO_WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapWhen(t => {
                return t.Request.Path.StartsWithSegments(new PathString("/api/token"));
            }, t => {
                OAuthConfig.Configure(t);
            });

            // Use cookies for logging in
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(Paths.LOGIN_PATH),
                LogoutPath = new PathString(Paths.LOGOUT_PATH),
                ExpireTimeSpan = TimeSpan.FromMinutes(15)
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
