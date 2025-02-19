using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using Punim_Diplome.Data;
using Punim_Diplome.Models;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using Punim_Diplome.Data.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;




namespace Punim_Diplome.Controllers
{

    public class ProduktController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly IKomentService _commentService;
        private readonly IProduktService _produktService;
        private readonly UserManager<IdentityUser> _userManager;




        public ProduktController(ApplicationDbContext context, IWebHostEnvironment environment, IKomentService komentService, IProduktService produktService, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.environment = environment;
            this._commentService = komentService;
            this._produktService = produktService;
            _userManager = userManager;

        }


        // GET: ProduktController
        [Authorize(Policy = "AdminEmail")]
        public async Task<IActionResult> Index(String searchString, String brand, String selectedCategory)
        {
            
            var produktetQuery = context.Produktet.OrderByDescending(p => p.Id).AsQueryable();


            if (!string.IsNullOrEmpty(brand))
            {
                produktetQuery = produktetQuery.Where(p => p.Brand == brand);
            }
            var availableBrands = await context.Produktet
               .Select(p => p.Brand)
               .Distinct()
               .ToListAsync();

            //SearchString
            if (!string.IsNullOrEmpty(searchString))
            {
                produktetQuery = produktetQuery.Where(p => p.Name!.Contains(searchString));
            }

            //Category
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                produktetQuery = produktetQuery.Where(p => p.Category!.Contains(selectedCategory));
            }

           
           var availableCategories = await context.Produktet
               .Select(p => p.Category)
               .Distinct()
               .ToListAsync();

            var viewModel = new ProductVM()
            {
                Products = await produktetQuery.ToListAsync(),
                SearchString = searchString,
                SelectedBrand = brand,
                AvailableBrands = availableBrands,
                SelectedCategory = selectedCategory,
                AvailableCategories = availableCategories
            };

            return View(viewModel);
        }

        [Authorize(Policy ="AdminEmail")]

        public async Task< IActionResult> Management(int id)

        {
            var produkt = await context.Produktet.FindAsync(id);
            if (produkt == null)
            {
                return RedirectToAction("Index", "Produkt");
            }
            return View(produkt);
        }
        

        [Authorize(Policy = "AdminEmail")]
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }


        [Authorize(Policy = "AdminEmail")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Brand,Description,Ram,Price,Date,Storage,Storagetype,ScreenInch,Procesor,Category,ImageFile")] ProduktDto produktDto)
        {
            if (produktDto.ImageFile != null)
            {
                string uploadDir = Path.Combine(environment.WebRootPath, "Produktet");
                string fileName = produktDto.ImageFile.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    produktDto.ImageFile.CopyTo(fileStream);
                }
                Produkt produkt = new Produkt()
                {
                    Name = produktDto.Name,
                    Brand = produktDto.Brand,
                    Ram = produktDto.Ram,
                    Price = produktDto.Price,
                    Storage = produktDto.Storage,
                    Storagetype = produktDto.Storagetype,
                    ScreenInch = produktDto.ScreenInch,
                    Procesor = produktDto.Procesor,
                    Category = produktDto.Category,
                    Date = produktDto.Date,
                    Description = produktDto.Description,
                    ImageFileName = fileName



                };
                context.Produktet.Add(produkt);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Produkt");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Brand,Description,Ram,Price,Date,Storage,Storagetype,ScreenInch,Procesor,Category,ImageFile")] ProduktDto produktDto)
        {
            var produkti = await context.Produktet.FindAsync(id);
            if (produkti == null)
            {
                return RedirectToAction("Index", "Produkt");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Produkti.Id"] = id;
                ViewData["Produkti.newFileName"] = produkti.ImageFileName;
                return View(produktDto);
            }


            produkti.Id = produktDto.Id;
            produkti.Name = produktDto.Name;
            produkti.Brand = produktDto.Brand;
            produkti.Description = produktDto.Description;
            produkti.Ram = produktDto.Ram;
            produkti.Price = produktDto.Price;
            produkti.Date = produktDto.Date;
            produkti.Storage = produktDto.Storage;
            produkti.Storagetype = produktDto.Storagetype;
            produkti.ScreenInch = produktDto.ScreenInch;
            produkti.Procesor = produktDto.Procesor;
            produkti.Category = produktDto.Category;





            if (produktDto.ImageFile != null && produktDto.ImageFile.Length > 0)
            {

                string newFileName = Guid.NewGuid() + Path.GetExtension(produktDto.ImageFile.FileName);

                // Define the full path to save the image
                string imageFullPath = Path.Combine(environment.WebRootPath, "Produktet", newFileName);

                // Save the new image
                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await produktDto.ImageFile.CopyToAsync(stream);
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(produkti.ImageFileName))
                {
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "Produktet", produkti.ImageFileName);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }


                produkti.ImageFileName = newFileName;
            }

            // Save changes to the database
            context.Produktet.Update(produkti);
            await context.SaveChangesAsync();


            return RedirectToAction("Index", "Produkt");
        }

        [Authorize(Policy = "AdminEmail")]
        public async Task<IActionResult> Edit(int id)
        {
            var produkti = await context.Produktet.FindAsync(id);

            if (produkti == null)
            {
                return RedirectToAction("Index", "Produkt");
            }

            var produktDto = new ProduktDto()
            {
                Name = produkti.Name,
                Brand = produkti.Brand,
                Ram = produkti.Ram,
                Price = produkti.Price,
                Procesor = produkti.Procesor,
                ScreenInch = produkti.ScreenInch,
                Storage = produkti.Storage,
                Storagetype = produkti.Storagetype,
                Description = produkti.Description,
                Date = produkti.Date
            };
            ViewData["Produkti.Id"] = produkti.Id;
            ViewData["produktDto.OldImageFullPath"] = produkti.ImageFileName;

            return View(produktDto);
        }

        [HttpGet("delete/{id}")]

        public async Task<IActionResult> Delete(int? id)
        {
            var produkti = context.Produktet.Find(id);
            string ImageFullPath = Path.Combine(environment.WebRootPath + "Produktet" + produkti.ImageFileName);

            if (produkti == null)
            {
                return RedirectToAction("Index", "Produkt");

            }




            if (System.IO.File.Exists(ImageFullPath))
            {
                System.IO.File.Delete(ImageFullPath);
            }

            context.Remove(produkti);

            await context.SaveChangesAsync();

            return RedirectToAction("Index", "Produkt");

        }


        [Authorize(Policy = "AdminEmail")]
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteConfirmation(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var produkti = await context.Produktet.FindAsync(id);
            if (produkti == null)
            {
                return RedirectToAction("Index");
            }


            string imageFullPath = Path.Combine(environment.WebRootPath, "Produktet", produkti.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }
            context.Produktet.Remove(produkti);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Details(int? id, Koment koment)
        {

            var produkt = await context.Produktet
       .Include(p => p.Koments)
       .ThenInclude(k => k.User) 
       .FirstOrDefaultAsync(p => p.Id == id);


            if (produkt == null)
            {
                return NotFound();
            }
            var viewModel = new ProduktDetailsVM()
            {
                Produkt = produkt,
                Koments = produkt.Koments?.ToList() ?? new List<Koment>(),
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int produktId, string content)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("Content", "Comment cannot be empty.");
                return RedirectToAction("Details", new { id = produktId });
            }

            // Get the logged-in user's ID
            var userId = _userManager.GetUserId(User);
            Console.WriteLine(userId);
            if (userId == null)
            {
                return Unauthorized();
            }


            var comment = new Koment
            {
                Content = content,
                IdentityUserId = userId,
                ProduktId = produktId
            };

            // Add the comment to the database
            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            // Redirect back to the details page for the product
            return RedirectToAction("Details", new { id = produktId });
        }

        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound("The comment does not exist.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment.IdentityUserId != userId)
            {
                return Unauthorized("You are not authorized to delete this comment.");
            }

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();

            // Ensure ProduktId is not null before redirecting
            if (comment.ProduktId == null)
            {
                // Handle the case where ProduktId is null, maybe redirect somewhere else
                return RedirectToAction("Index", "Home"); // Or another page
            }

            return RedirectToAction("Details", "Produkt", new { id = comment.ProduktId });
        }




        public async Task<IActionResult> PrivateAdmin()
        {
            var AllUser = await context.Users.ToListAsync();

            return View(AllUser);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("PrivateAdmin");
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction("PrivateAdmin");
        }

        



    }
 
    

}
