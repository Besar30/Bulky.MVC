
using Bulky.Data.Models;
using Bulky.infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;


namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.categoryRepository.GetAll().OrderBy(x=>x.DisplayOrder).ToList();
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
                _unitOfWork.categoryRepository.Add(category);
                _unitOfWork.save();
                TempData["success"] = "Category created successflly.";
                return RedirectToAction("Index");
            }
            return View("Create", category);
        }
        public IActionResult Edit(int? id)
        {
            if (id==null ||id==0)
            {
                return NotFound();
            }
            Category? category= _unitOfWork.categoryRepository.Get(x=>x.Id == id);
            if (category == null) {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost()]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                Category? cate = _unitOfWork.categoryRepository.Get(x => x.Id == category.Id);
                if (cate == null) {
                    return NotFound();
                }
                cate.Name=category.Name;
                cate.DisplayOrder=category.DisplayOrder;
                _unitOfWork.save();
                TempData["success"] = "Category updated successflly.";
                return RedirectToAction("Index");
            }
            return View("Edit", category);
        }
        public  IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.categoryRepository.Get(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost()]
        public IActionResult DeleteCategory(int id)
        {
                Category? cate = _unitOfWork.categoryRepository.Get(x => x.Id == id);
            if (cate == null) {
                return NotFound();
            }
            _unitOfWork.categoryRepository.Remove(cate);
            _unitOfWork.save();
            TempData["success"] = "Category deleted successflly.";
            return RedirectToAction("Index");
        }
    }
}
