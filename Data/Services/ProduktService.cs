using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Models;

namespace Punim_Diplome.Data.Services
{
    public class ProduktService : IProduktService
    {
        private readonly ApplicationDbContext _context;
        public ProduktService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<Produkt> GetByID(int id)
        {
            var Produktet = await _context.Produktet.Include(p => p.Koments).ThenInclude(k => k.User).FirstAsync(m => m.Id == id);
            return Produktet;
        }
    }
}
