using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserSportsController : ControllerBase
    {
        private readonly FitnessDBContext _context;

        public UserSportsController(FitnessDBContext context)
        {
            _context = context;
        }

        // GET: api/UserSports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSport>>> GetUserSport()
        {
            return await _context.UserSport.ToListAsync();
        }

        // GET: api/UserSports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSport>> GetUserSport(string id)
        {
            var userSport = await _context.UserSport.FindAsync(id);

            if (userSport == null)
            {
                return NotFound();
            }

            return userSport;
        }

        // PUT: api/UserSports/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSport(string id, UserSport userSport)
        {
            if (id != userSport.SportName)
            {
                return BadRequest();
            }

            _context.Entry(userSport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSportExists(id))
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

        // POST: api/UserSports
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserSport>> PostUserSport(UserSport userSport)
        {
            _context.UserSport.Add(userSport);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserSportExists(userSport.SportName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserSport", new { id = userSport.SportName }, userSport);
        }

        // DELETE: api/UserSports/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserSport>> DeleteUserSport(string id)
        {
            var userSport = await _context.UserSport.FindAsync(id);
            if (userSport == null)
            {
                return NotFound();
            }

            _context.UserSport.Remove(userSport);
            await _context.SaveChangesAsync();

            return userSport;
        }

        private bool UserSportExists(string id)
        {
            return _context.UserSport.Any(e => e.SportName == id);
        }
    }
}
