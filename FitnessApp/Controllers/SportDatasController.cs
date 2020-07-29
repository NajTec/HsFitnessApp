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
   // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SportDatasController : ControllerBase
    {
        private readonly FitnessDBContext _context;

        public SportDatasController(FitnessDBContext context)
        {
            _context = context;
        }

        // GET: api/SportDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SportData>>> GetSportData()
        {
            return await _context.SportData.ToListAsync();
        }

   

        // GET: api/SportDatas/5
        [HttpGet("{name}")]
        public async Task<ActionResult<SportData>> GetSportData(string name)
        {
            var sportData = await _context.SportData.FindAsync(name);

            if (sportData == null)
            {
                return NotFound();
            }

            return sportData;
        }

        // PUT: api/SportDatas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSportData(string id, SportData sportData)
        {
            if (id != sportData.Name)
            {
                return BadRequest();
            }

            _context.Entry(sportData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SportDataExists(id))
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

        // POST: api/SportDatas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<SportData>> PostSportData(SportData sportData)
        {
            _context.SportData.Add(sportData);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SportDataExists(sportData.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSportData", new { id = sportData.Name }, sportData);
        }

        // DELETE: api/SportDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SportData>> DeleteSportData(string id)
        {
            var sportData = await _context.SportData.FindAsync(id);
            if (sportData == null)
            {
                return NotFound();
            }

            _context.SportData.Remove(sportData);
            await _context.SaveChangesAsync();

            return sportData;
        }

        private bool SportDataExists(string id)
        {
            return _context.SportData.Any(e => e.Name == id);
        }
    }
}
