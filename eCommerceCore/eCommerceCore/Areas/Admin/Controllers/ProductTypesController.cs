using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceCore.Data;
using eCommerceCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceCore.Areas.Customer.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            //populate
            return View(_db.ProductTypes.ToList());
        }

        //Create Get Action Method
        public IActionResult Create()
        {
            return View();
        }

        //POST Create action Method
        [HttpPost]
        [ValidateAntiForgeryToken] //check if the tokens is valid or not
        //push action method
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Add(productTypes);
                await _db.SaveChangesAsync();
                TempData["message"] = "Data has been Added to Database.";
                return RedirectToAction(nameof(Index));

            }
            return View(productTypes);
        }

        //Edit Method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //EDIT action Method
        [HttpPost]
        [ValidateAntiForgeryToken] //check if the tokens is valid or not
        //push action method
        public async Task<IActionResult> Edit(int id, ProductTypes productTypes)
        {
            if (id != productTypes.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                _db.Update(productTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }


        //Get Details Method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }


        //Delete Method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //Delete action Method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] //check if the tokens is valid or not
        //push action method
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productTypes = await _db.ProductTypes.FindAsync(id);
            _db.ProductTypes.Remove(productTypes);
            await _db.SaveChangesAsync();
            TempData["message"] = "Data has been Deleted !!!";
            return RedirectToAction(nameof(Index));
        }
    }
}