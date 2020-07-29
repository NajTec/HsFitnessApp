using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FitnessApp.Controllers
{
    [Authorize] // -> Zugriff auf die Web-API nur mit Bearer-Header
    [Route("api/[controller]")]
    [ApiController]
    public class UserFoodsController : ControllerBase
    {
        
        private readonly FitnessDBContext _context;
        private readonly JWTSettings _jWTSettings;
        private readonly FitnessDBWestContext _westcontext;

        public UserFoodsController(FitnessDBContext context,FitnessDBWestContext westContext ,IOptions<JWTSettings> jwtsettings)
        {
            _context = context;
            _jWTSettings = jwtsettings.Value;
            _westcontext = westContext;
        }

        // GET: api/UserFoods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserFood>>> GetUserFood()
        {
            return await _context.UserFood.ToListAsync();
        }

        [HttpGet("personalFood/{accesstoken}")]
        public async Task<ActionResult<IEnumerable<UserFood>>> GetUsersFood(string accesstoken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTSettings.SecretKey);
            var accessToken = accesstoken;

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
                var region = principal.FindFirst(ClaimTypes.Locality)?.Value;

                var userData = _context.UserData.Where(user => user.Alias == alias && user.Region == region).FirstOrDefault(); //schaut in der ersten API ob es in Nordeuropa ist

                if (userData == null) { 
                    userData =  _westcontext.UserData.Where(user => user.Alias == alias && user.Region == region).FirstOrDefault();
                }

                if ((userData == null)) {
                    return NotFound();
                }

                return await _context.UserFood.Where(user => user.UserAlias == alias).ToListAsync(); //gib nur das Essen zurück des Users mit dem Token der das richtige Token mit der richtigen Region hat
            }


            return null;
        }

        /*
        // GET: api/UserFoods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserFood>> GetUserFood(string id)
        {
            var userFood = await _context.UserFood.FindAsync(id);

            if (userFood == null)
            {
                return NotFound();
            }

            return userFood;
        } */

        // PUT: api/UserFoods/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserFood(string id, UserFood userFood)
        {
            if (id != userFood.UserAlias)
            {
                return BadRequest();
            }

            _context.Entry(userFood).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserFoodExists(id))
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

        // POST: api/UserFoods
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserFood>> PostUserFood(UserFood userFood)
        {
            _context.UserFood.Add(userFood);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserFoodExists(userFood.FoodName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserFood", new { id = userFood.FoodName }, userFood);
        }

        // DELETE: api/UserFoods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserFood>> DeleteUserFood(string id)
        {
            var userFood = await _context.UserFood.FindAsync(id);
            if (userFood == null)
            {
                return NotFound();
            }

            _context.UserFood.Remove(userFood);
            await _context.SaveChangesAsync();

            return userFood;
        }

        private bool UserFoodExists(string id)
        {
            return _context.UserFood.Any(e => e.FoodName == id);
        }
    }
}
