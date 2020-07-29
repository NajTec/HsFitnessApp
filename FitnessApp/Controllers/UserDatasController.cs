using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using System.Security.Cryptography;
using System.Data.Entity.Core.Metadata.Edm;

namespace FitnessApp.Controllers
{
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserDatasController : ControllerBase
    {
        private readonly FitnessDBContext _context;
        private readonly FitnessDBWestContext _westcontext;
        private readonly JWTSettings _jWTSettings;
  
       
        public UserDatasController(FitnessDBContext context, FitnessDBWestContext westcontext, IOptions<JWTSettings> jwtsettings)
        {
            _context = context;
            _westcontext = westcontext;
            _jWTSettings = jwtsettings.Value;
        }

        // GET: api/UserDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserData>>> GetUserData()
        {
          //  string region = Environment.GetEnvironmentVariable("REGION_NAME"); //funktioniert nur wenn man web api als web service implementiert
            var users = await _context
                .UserData
                .Where(users => users.Region == "northeurope") //weil region nur funktioniert als appservice
                .ToListAsync(); 
            if (users == null) { // wenn der jetztige User nicht in dieser  DB ist, in der anderen Datenbank abfragen
                users = await _westcontext
                        .UserData
                        .Where(users => users.Region == "westeurope")
                        .ToListAsync();  
            } 
            
            return users;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<FitnessUserWithToken>> Login([FromBody] UserData user) {
            user = await _context.UserData
                    .Where(u => u.Alias == user.Alias && u.Password == user.Password )
                    .FirstOrDefaultAsync();
            // wenn nicht schaue andere DB
           // user = await _westcontext.UserData
           //        .Where(u => u.Alias == user.Alias && u.Password == user.Password && u.region == "westeurope")
           //        .FirstOrDefaultAsync();
            FitnessUserWithToken userWithToken = null;

            if (user != null) {     
                RefreshToken refreshToken = GenerateRefreshToken();
                user.RefreshToken.Add(refreshToken);

                await _context.SaveChangesAsync();
                userWithToken = new FitnessUserWithToken(user);
                userWithToken.RefreshToken = refreshToken.Token;
            }

           

            if (userWithToken == null)
            {
                return NotFound();
            }  
            
            //generate AccessToken
            userWithToken.AccessToken = GenerateAccessToken(user);

            return userWithToken;
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<FitnessUserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest) {
            UserData user = GetUserFromAccessToken(refreshRequest.AccessToken);

            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken)) {
                FitnessUserWithToken userWithToken = new FitnessUserWithToken(user);
                userWithToken.AccessToken = GenerateAccessToken(user);

                return userWithToken;
            }

            return NotFound();
        }

        private bool ValidateRefreshToken(UserData user, string refreshToken)
        {
           RefreshToken  refreshTokenUser = _context.RefreshToken.Where(refreshtk => refreshtk.Token == refreshToken)
                                    .OrderByDescending(rt => rt.ExpiryDate)
                                    .FirstOrDefault();

            if (refreshTokenUser != null && refreshTokenUser.Alias == user.Alias && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }

            else return false;
        }

        private UserData GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jWTSettings.SecretKey);


                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;

                var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var alias = principal.FindFirst(ClaimTypes.Name)?.Value;

                    var userData  = _context.UserData.Where(user => user.Alias == alias).FirstOrDefault();

                    if (userData == null) {
                        userData = _westcontext.UserData.Where(user => user.Alias == alias).FirstOrDefault();
                    }

                    return userData;
                }
            }
            catch (Exception e) {
                
            }
            return null;
        }

        private RefreshToken GenerateRefreshToken() {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomNumber);
                refreshToken.Token= Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

            return refreshToken;
        }

        private string GenerateAccessToken(UserData user) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTSettings.SecretKey);  //normally Azure Key Vault Vault
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Alias),
                    new Claim(ClaimTypes.Locality, user.Region),
                }),
                Expires = DateTime.UtcNow.AddSeconds(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    

       // api/UserDatas/GetRegion
        [HttpGet("GetRegion")]
        public async Task<string> GetRegion() {
            string region ="westeurope";

            return region;
        }

        /*
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserData>> GetUser() {

            string alias = HttpContext.User.Identity.Name;

            var user = await _context.UserData
                    .Where(user => user.Alias == alias)
                    .FirstOrDefaultAsync();

            if (user == null) {
                return NotFound();
                    }

            return user;
        
        } */

        // GET: api/UserDatas/5
        [HttpGet("GetUserDataDetails/{alias}")]
        public async Task<ActionResult<UserData>> GetUserData(string alias)
        {
            var userData = _westcontext.UserData
                .Include(userfood => userfood.UserFood)
                .Include(usersport => usersport.UserSport)
                .Where(user => user.Alias == alias)
                .FirstOrDefault();

            if (userData == null)
            {
               userData = _context.UserData //suche in anderer Datenbank
                .Include(userfood => userfood.UserFood)
                .Include(usersport => usersport.UserSport)
                .Where(user => user.Alias == alias)
                .FirstOrDefault(); 
            }

            if (userData == null) {
                return NotFound();
            }

            return userData;
        }

        // PUT: api/UserDatas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserData(string id, UserData userData)
        {
            if (id != userData.Alias)
            {
                return BadRequest();
            }

            _context.Entry(userData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserDatas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserData>> PostUserData(UserData userData)
        {
            _context.UserData.Add(userData);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserDataExists(userData.Alias))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserData", new { id = userData.Alias }, userData);
        }

        // DELETE: api/UserDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserData>> DeleteUserData(string id)
        {
            var userData = await _context.UserData.FindAsync(id);
            if (userData == null)
            {
                return NotFound();
            }

            _context.UserData.Remove(userData);
            await _context.SaveChangesAsync();

            return userData;
        }

        private bool UserDataExists(string id)
        {
            return _context.UserData.Any(e => e.Alias == id);
        }
    }
}
