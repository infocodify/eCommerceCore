using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Data;
using eCommerceCore.Models;
using eCommerceCore.Models.ViewModel;
using eCommerceCore.Utility;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerceCore.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProducsVM { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;

            _hostingEnvironment = hostingEnvironment;

            //initialize ProductsViewModel
            ProducsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }

        //Get : Products Create
        public IActionResult Create()
        {
            return View(ProducsVM);
        }

        //Post : Products Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProducsVM);
            }

            _db.Products.Add(ProducsVM.Products);
            await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _db.Products.Find(ProducsVM.Products.Id);

            if (files.Count != 0)
            {
                //Image has been Uploaded

                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using(var filestream = new FileStream(Path.Combine(uploads, ProducsVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProducsVM.Products.Id + extension;
            }
            //if no file uploaded
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProducImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProducsVM.Products.Id + ".jpg");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProducsVM.Products.Id + ".jpg";
            }

            await _db.SaveChangesAsync();
            TempData["message"] = "Data has been Added to Database.";
            return RedirectToAction(nameof(Index));
        }

        //GET : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProducsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProducsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProducsVM);
        }


        //Post : Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.Id == ProducsVM.Products.Id).FirstOrDefault();

                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, ProducsVM.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProducsVM.Products.Id + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, ProducsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    ProducsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProducsVM.Products.Id + extension_new;
                }

                if (ProducsVM.Products.Image != null)
                {
                    productFromDb.Image = ProducsVM.Products.Image;
                }

                productFromDb.Name = ProducsVM.Products.Name;
                productFromDb.Price = ProducsVM.Products.Price;
                productFromDb.Available = ProducsVM.Products.Available;
                productFromDb.Image = ProducsVM.Products.Image;
                productFromDb.ProductTypeId = ProducsVM.Products.ProductTypeId;
                productFromDb.SpecialTagsID = ProducsVM.Products.SpecialTagsID;
                productFromDb.ShadeColor = ProducsVM.Products.ShadeColor;
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ProducsVM);
        }


        //GET : Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProducsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProducsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProducsVM);
        }

        //GET : Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProducsVM.Products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProducsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProducsVM);
        }

        //POST : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(products.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
                }
                _db.Products.Remove(products);
                await _db.SaveChangesAsync();
                TempData["message"] = "Data has been Deleted Successfully !!!";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}