using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Models;
using Punim_Diplome.Data.Services;

namespace Punim_Diplome.Controllers
{
    public class OrderController : Controller


    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager  )
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()

        {

            var userID = _userManager.GetUserId(User);


            var orders =  await _context.OrderProducts
                .Include(o => o.Produkt)
                .Where(o => o.UserId == userID)
                .ToListAsync();



            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> OrderNow(int id)
        {
            var userID = _userManager.GetUserId(User);

            if (userID == null)
            {
                return Unauthorized();
            }
          

            var product =  await _context.Produktet.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var order = new OrderProduct
            {
                UserId = userID,
                ProductId = id,
                OrderDate = DateTime.Now

            };
             await _context.OrderProducts.AddAsync(order);
            await _context.SaveChangesAsync();


          
            return RedirectToAction("Index");

        }

        [HttpDelete]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userID = _userManager.GetUserId(User);
            if (userID == null)
            {
                return Unauthorized();
            }
            var order = await _context.OrderProducts
                .Where(o => o.UserId == userID && o.ProductId == id)
                .FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }
            _context.OrderProducts.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminEmail")]
        public async Task<IActionResult> AllOrders()
        {
           var oders = await _context.OrderProducts
                .Include(o => o.Produkt)
                .Include(o => o.User)
                .ToListAsync();

            return View(oders);
        }

    }
}
