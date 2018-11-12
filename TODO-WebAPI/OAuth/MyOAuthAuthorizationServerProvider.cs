using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace TODO_WebAPI.OAuth
{
    public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string ClientID = string.Empty;
            string ClientSecret = string.Empty;

            if(!context.TryGetBasicCredentials(out ClientID, out ClientSecret))
            {
                context.TryGetFormCredentials(out ClientID, out ClientSecret);
             }

            using (var db = new Models.TodoAppDbContext())
            {
                var client = db.Clients.SingleOrDefault(t => t.Client_ID == ClientID);

                // TODO - Security: Authorize SPA clients without asking for client secret.
                // Secret should not be kept in SPA application for security reason
                if(client.Client_Secret == ClientSecret)
                {
                    context.Validated();
                } else {
                    context.SetError("invalid_clientid", "Incorrect Client ID or Client Secret.");
                }
            }

            context.OwinContext.Set<string>("as:client_id", context.ClientId);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (var db = new Models.TodoAppDbContext())
            {
                var user = db.Users.SingleOrDefault(t => t.Username == context.UserName);
                if (Crypto.VerifyHashedPassword(user.Password, context.Password))
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaims(new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, context.UserName),
                            new Claim("as:client_id", context.ClientId)
                        });

                    if (!string.IsNullOrEmpty(context.Scope.FirstOrDefault()))
                    {
                        identity.AddClaims(context.Scope.First()?.Split(',')?.Select(t => new Claim("as:scope", t)));
                    }

                    var props = new AuthenticationProperties(new Dictionary<string, string>
                        {
                            { "client_id", context.ClientId },
                            { "username", context.UserName }
                        });

                    var ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
                }
                else
                {
                    context.Rejected();
                    context.SetError("invalid_grant", "Username or Password is not correct.");
                }
            }
        }

        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
        }
    }
}