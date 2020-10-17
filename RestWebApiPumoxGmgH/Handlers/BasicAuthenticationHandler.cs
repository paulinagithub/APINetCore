using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RestWebApiPumoxGmgH.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string _email;
        private readonly string _password;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _email = "janKowalski@gmail.com";
            _password = "jankowalski";
        }
        //Authorization Header base64= basic amFuS293YWxza2lAZ21haWwuY29tIGphbmtvd2Fsc2tp 
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Authorization header was not found");

            try
            {
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                if(credentials.Count() == 2)
                {
                    string emailAddress = credentials[0];
                    string password = credentials[1];
                    if (emailAddress == _email && password == _password)
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, emailAddress) };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);

                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalid username or password");
                    }
                }  
                else
                {
                    return AuthenticateResult.Fail("Something went wrong!");
                }
            }
            catch(Exception)
            {
                return AuthenticateResult.Fail("Error has occured");
            }
        }
    }
}
