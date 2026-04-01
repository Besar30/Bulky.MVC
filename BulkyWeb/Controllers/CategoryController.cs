using BulkyWeb.DataBase;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BulkyWeb.Controllers
{
    public class CategoryController(ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            List<Category> categories = context.Categories.OrderBy(x=>x.DisplayOrder).ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost()]
        public IActionResult Create(Category category) {
            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                context.SaveChanges();
                TempData["success"] = "Category created successflly.";
                return RedirectToAction("Index");
            }
            return View("Create", category);
        }
        public  async  Task<IActionResult> Edit(int? id)
        {
            if (id==null ||id==0)
            {
                return NotFound();
            }
            Category? category=await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);
            if (category == null) {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost()]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                Category? cate = await context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
                if (cate == null) {
                    return NotFound();
                }
                cate.Name=category.Name;
                cate.DisplayOrder=category.DisplayOrder;
                await context.SaveChangesAsync();
                TempData["success"] = "Category updated successflly.";
                return RedirectToAction("Index");
            }
            return View("Edit", category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost()]
        public async Task<IActionResult> DeleteCategory(int id)
        {
                Category? cate = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (cate == null) {
                return NotFound();
            }
                context.Categories.Remove(cate);
                await context.SaveChangesAsync();
            TempData["success"] = "Category deleted successflly.";
            return RedirectToAction("Index");
        }
    }
}
