using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class StatesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatesController(DataContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAsync()
        //{
        //    return Ok(await _context.States
        //                            .Include(x => x.Cities)
        //                           .ToListAsync());
        //}

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.States
                .Include(x => x.Cities)
                .Where(x => x.Country!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync());
        }

        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.States
                .Where(x => x.Country!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        //[HttpGet("id:int")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await _context
                            .States
                            .Include(x => x.Cities)
                            .FirstOrDefaultAsync(x => x.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(State state)
        {
            try
            {
                _context.Add(state);
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("ya existe una provincia con el mismo nombre");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest($"Provincia {exception.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(State state)
        {
            try
            {
                _context.Update(state);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicada"))
                {
                    return BadRequest("ya existe una provincia con el mismo nombre");
                }

                return BadRequest(dbUpdateException.Message);
            }
            catch (Exception exception)
            {
                return BadRequest($"Provincia {exception.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var state = await _context.States.FirstOrDefaultAsync(x => x.Id == id);
            if (state == null)
            {
                return NotFound();
            }
            _context.Remove(state);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}