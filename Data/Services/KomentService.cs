using DocumentFormat.OpenXml.Office2021.PowerPoint.Comment;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Data.Services;
using Punim_Diplome.Models;

namespace Punim_Diplome.Data.Services

{
    public class KomentService : IKomentService
    {
        private readonly ApplicationDbContext _context;


        public KomentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public  async Task Add(Koment koment)
        {
            _context.Comments.Add(koment);

            await  _context.SaveChangesAsync();
        }

        public Task GetCommentById(int id)
        {
            return _context.Comments.FirstOrDefaultAsync(koment => koment.Id == id);
        }
    }
}
