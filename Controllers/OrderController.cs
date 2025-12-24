using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Models;
using Punim_Diplome.Data.Services;
using System.Security.Claims;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Punim_Diplome.Controllers
{
    public class OrderController : Controller


    {
        private readonly ILogger<OrderController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            
            _context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()

        {

            var userID = _userManager.GetUserId(User);


            var orders = await _context.OrderProducts
                .Include(o => o.Produkt)
                .Where(o => o.UserId == userID)
                .ToListAsync();



            return View(orders);
        }

        //[HttpPost]
        //public async Task<IActionResult> OrderNow(int id)
        //{
        //    var userID = _userManager.GetUserId(User);

        //    if (userID == null)
        //    {
        //        return Unauthorized();
        //    }


        //    var product = await _context.Produktet.FindAsync(id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = new OrderProduct
        //    {
        //        UserId = userID,
        //        ProductId = id,
        //        OrderDate = DateTime.Now

        //    };
        //    await _context.OrderProducts.AddAsync(order);
        //    await _context.SaveChangesAsync();



        //    return RedirectToAction("Index");

        //}

        [HttpPost]
        [ValidateAntiForgeryToken] // You can remove IgnoreAntiforgeryToken now
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userID = _userManager.GetUserId(User);

            var orderproduct = await _context.OrderProducts
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userID);

            if (orderproduct == null)
            {
                return NotFound();
            }

            _context.OrderProducts.Remove(orderproduct);
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
        [HttpGet]
        public async Task< IActionResult> OrderForm(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _context.Produktet.FindAsync(productId);
            var model = new OrderProduct
            {
                ProductId = productId,
                UserId = userId,
                Produkt = product

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderForm(OrderProduct order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            order.OrderDate = DateTime.Now;

            _context.OrderProducts.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Order"); // Or a confirmation page
        }

        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.OrderProducts.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            
            order.OrderStatus = status;

            _context.OrderProducts.Update(order);

            await _context.SaveChangesAsync();


            return RedirectToAction("AllOrders");


        }



    }

}
