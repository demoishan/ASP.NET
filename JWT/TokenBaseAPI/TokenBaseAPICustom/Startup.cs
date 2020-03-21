using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using TokenBaseAPICustom.Providers;

[assembly: OwinStartup(typeof(TokenBaseAPICustom.Startup))]

namespace TokenBaseAPICustom
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthCustomeTokenProvider(), // We will create
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(20),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new OAuthCustomRefreshTokenProvider() // We will create
            };
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
