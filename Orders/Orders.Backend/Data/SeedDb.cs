using Microsoft.EntityFrameworkCore;
using Orders.Shared.Entities;

namespace Orders.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckContruiesAsync();
        }

        private async Task CheckContruiesAsync()
        {
            //Si no hay registros
            if (!_context.Countries.Any())
            {
                _context.Add(new Country { Name = "Argentina" });
                _context.Add(new Country { Name = "Colombia" });
                _context.Add(new Country { Name = "Perü" });
                await _context.SaveChangesAsync();
            }
        }
    }
}