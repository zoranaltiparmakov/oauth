using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TODO_WebAPI.OAuth;

namespace TODO_WebAPI.App_Start
{
    public class OAuthConfig
    {
        public static void Configure(IAppBuilder app)
        {
            // Setup Auth Server
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString(Paths.TOKEN_PATH),
                ApplicationCanDisplayErrors = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(15),
                Provider = new MyOAuthAuthorizationServerProvider(),
            });
        }
    }
}