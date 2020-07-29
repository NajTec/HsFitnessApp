using FitnessApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FitnessApp.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private readonly FitnessDBContext _context;
        private readonly FitnessDBWestContext _westcontext;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            FitnessDBContext context,
            FitnessDBWestContext westcontext)
                : base(options, logger, encoder, clock)
        {
            _context = context;
            _westcontext = westcontext;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) {
                return AuthenticateResult.Fail("Authorization header was not found ");
            }

           var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            try
            {
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
                string alias = credentials[0];
                string password = credentials[1];
                

                UserData user = _context.UserData.Where(user => user.Alias == alias && user.Password == password).FirstOrDefault();

                if (user == null) {
                   user = _westcontext.UserData.Where(user => user.Alias == alias && user.Password == password).FirstOrDefault();
                }

                if (user == null)
                    AuthenticateResult.Fail("Invalid username or password");
                else {
                    var claims = new[] { new Claim(ClaimTypes.Name, user.Alias) };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                   return AuthenticateResult.Success(ticket);
                }
            }

            catch (Exception) {

                return AuthenticateResult.Fail("Error");
            }
            return AuthenticateResult.NoResult();
        }
    }
}
