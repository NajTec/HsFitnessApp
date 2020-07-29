using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Models;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodDatasController : ControllerBase
    {
        private readonly FitnessDBContext _context;

        public FoodDatasController(FitnessDBContext context)
        {
            _context = context;
        }

        // GET: api/FoodDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodData>>> GetFoodData()
        {
            return await _context.FoodData.ToListAsync();
        }

        // GET: api/FoodDatas/5
        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<FoodData>>> GetFoodData(string name)
        {
            var foodData = await _context.FoodData.Where(food => food.Name == name).ToListAsync();

            if (foodData == null)
            {
                return NotFound();
            }

            return foodData;
        }

        // PUT: api/FoodDatas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodData(string id, FoodData foodData)
        {
            if (id != foodData.Name)
            {
                return BadRequest();
            }

            _context.Entry(foodData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodDataExists(id))
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

        // POST: api/FoodDatas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FoodData>> PostFoodData(FoodData foodData)
        {
            _context.FoodData.Add(foodData);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FoodDataExists(foodData.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFoodData", new { id = foodData.Name }, foodData);
        }

        // DELETE: api/FoodDatas/5
        [HttpDelete("{name}")]
        public async Task<ActionResult<FoodData>> DeleteFoodData(string name)
        {
            var foodData = await _context.FoodData.FindAsync(name);
            if (foodData == null)
            {
                return NotFound();
            }

            _context.FoodData.Remove(foodData);
            await _context.SaveChangesAsync();

            return foodData;
        }

        private bool FoodDataExists(string name)
        {
            return _context.FoodData.Any(e => e.Name == name);
        }
    }
}
