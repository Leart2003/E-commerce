using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Punim_Diplome.Data;
using Punim_Diplome.Models;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using Punim_Diplome.Data.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using OfficeOpenXml;



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

        [Authorize(Policy = "AdminEmail")]

        public async Task<IActionResult> Menaxhment(String searchBar)

        {
            var produktet = await context.Produktet.OrderByDescending(p => p.Id).ToListAsync();

            if (produktet == null)
            {
                return NotFound();
            }
            if (!String.IsNullOrEmpty(searchBar))
            {
                produktet = produktet.Where(p => p.Name.ToUpper().Contains(searchBar.ToUpper())).ToList();
            }
            return View(produktet);
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
                    Id = produktDto.Id,
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


        [HttpGet]

        public IActionResult Edit(int id)
        {
            var produkti = context.Produktet.Find(id);

            if (produkti == null)
            {
                return RedirectToAction("Index", "Produkt");
            }

            var produktDto = new ProduktDto()
            {
                Id = produkti.Id,
                Name = produkti.Name,
                Brand = produkti.Brand,
                Ram = produkti.Ram,
                Description = produkti.Description,
                Date = produkti.Date,
                Price = produkti.Price,
                Storage = produkti.Storage,
                Procesor = produkti.Procesor,
                ScreenInch = produkti.ScreenInch,
                Storagetype = produkti.Storagetype



            };

            ViewData["ImageFileName"] = produkti.ImageFileName;
            ViewData["Produkt.Id"] = produkti.Id;
            return View(produktDto);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProduktDto produktDto)
        {
            var produkti = context.Produktet.Find(id);
            if (produkti == null)
            {
                return NotFound();
            }

            Console.WriteLine($"Found product: {produkti.Name}");

            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            // Update the fields with new values from the form
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

                string imageFullPath = Path.Combine(environment.WebRootPath, "Produktet", newFileName);

                using (var stream = new FileStream(imageFullPath, FileMode.Create))
                {
                    await produktDto.ImageFile.CopyToAsync(stream);
                }

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

            // Mark entity as modified and explicitly set each field as modified
            context.Entry(produkti).State = EntityState.Modified;
            context.Entry(produkti).Property(x => x.Name).IsModified = true;
            context.Entry(produkti).Property(x => x.Brand).IsModified = true;
            context.Entry(produkti).Property(x => x.Description).IsModified = true;
            context.Entry(produkti).Property(x => x.Ram).IsModified = true;
            context.Entry(produkti).Property(x => x.Price).IsModified = true;
            context.Entry(produkti).Property(x => x.Date).IsModified = true;
            context.Entry(produkti).Property(x => x.Storage).IsModified = true;
            context.Entry(produkti).Property(x => x.Storagetype).IsModified = true;
            context.Entry(produkti).Property(x => x.ScreenInch).IsModified = true;
            context.Entry(produkti).Property(x => x.Procesor).IsModified = true;
            context.Entry(produkti).Property(x => x.Category).IsModified = true;

            // Save changes to the database
            var changes = await context.SaveChangesAsync();
            Console.WriteLine($"Rows affected: {changes}");

            return RedirectToAction("Index", "Produkt");
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

        [HttpGet]
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

        public async Task<IActionResult> ExportToExcel()
        {
            



            var produktet = await context.Produktet.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Produktet");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Brand";
                worksheet.Cells[1, 4].Value = "Description";
                worksheet.Cells[1, 5].Value = "Ram";
                worksheet.Cells[1, 6].Value = "Price";
                worksheet.Cells[1, 7].Value = "Date";
                worksheet.Cells[1, 8].Value = "Storage";
                worksheet.Cells[1, 9].Value = "Storage Type";
                worksheet.Cells[1, 10].Value = "Screen Inch";
                worksheet.Cells[1, 11].Value = "Procesor";
                worksheet.Cells[1, 12].Value = "Category";
                worksheet.Cells[1, 13].Value = "Image File Name";

                int row = 2;

                foreach (var produkt in produktet)
                {
                    worksheet.Cells[row, 1].Value = produkt.Id;
                    worksheet.Cells[row, 2].Value = produkt.Name;
                    worksheet.Cells[row, 3].Value = produkt.Brand;
                    worksheet.Cells[row, 5].Value = produkt.Ram;
                    worksheet.Cells[row, 6].Value = produkt.Price;
                    worksheet.Cells[row, 7].Value = produkt.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 8].Value = produkt.Storage;
                    worksheet.Cells[row, 9].Value = produkt.Storagetype;
                    worksheet.Cells[row, 10].Value = produkt.ScreenInch;
                    worksheet.Cells[row, 11].Value = produkt.Procesor;
                    worksheet.Cells[row, 12].Value = produkt.Category;
                    worksheet.Cells[row, 13].Value = produkt.ImageFileName;
                    row++;
                }
                 worksheet.Cells.AutoFitColumns();
                 
                var stream = new MemoryStream();    
                package.SaveAs(stream);
                stream.Position = 0;
                
                string excelName = $"Produktet_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);

            }
        }
    }
}

 

