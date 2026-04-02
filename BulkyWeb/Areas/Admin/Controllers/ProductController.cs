using Bulky.Data.Mapping;
using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.Repository;
using Bulky.infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.productRepository.GetAllProduct().ToList();

            return View(products);
        }
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> Categorylist = _unitOfWork.categoryRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            ProductVM productVM = new ProductVM
            {
                product = new Product(),
                CategoryList = Categorylist
            };
            if(id == null)
            return View(productVM);
            else
            {
                productVM.product=_unitOfWork.productRepository.Get(x=>x.Id==id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productvm)
        {
            if (productvm.product.Id == 0 && productvm.Image == null)
            {
                ModelState.AddModelError("Image", "Please upload an image.");
            }
            if (ModelState.IsValid)
            {
                if (productvm.Image != null&&productvm.product.Id==0)
                {
                    string imagePath = SaveImage(productvm.Image, "Product");
                    productvm.product.ImageUrl = imagePath;
                }
                if (productvm.product.Id == 0)
                {
                    _unitOfWork.productRepository.Add(productvm.product);
                    TempData["success"] = "Product created successfully.";
                }
                else
                {
                    Product product = _unitOfWork.productRepository.Get(x => x.Id == productvm.product.Id);
                    productvm.ToEntity(product);
                    if (productvm.Image != null)
                    {
                        DeleteImage(product.ImageUrl!, "Product");
                        string imagePath = SaveImage(productvm.Image, "Product");
                        product.ImageUrl = imagePath;
                    }
                    TempData["success"] = "Product updated successfully.";
                }

                _unitOfWork.save();
                return RedirectToAction("Index");
            }

            IEnumerable<SelectListItem> Categorylist = _unitOfWork.categoryRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            ProductVM vm = new ProductVM
            {
                product = productvm.product,
                CategoryList = Categorylist
            };
            return View("Upsert", vm);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = _unitOfWork.productRepository.Get(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost()]
        public IActionResult DeleteProduct(int id)
        {
            Product? pro = _unitOfWork.productRepository.Get(x => x.Id == id);
            if (pro == null)
            {
                return NotFound();
            }
            DeleteImage(pro.ImageUrl, "Product");
            _unitOfWork.productRepository.Remove(pro);
            _unitOfWork.save();
            TempData["success"] = "Product deleted successflly.";
            return RedirectToAction("Index");
        }
        private string SaveImage(IFormFile image,string Section)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/{Section}");
            if(!Directory.Exists(UploadFolder))
                Directory.CreateDirectory(UploadFolder);
            string filePath=Path.Combine(UploadFolder, fileName);
            using(var stream=new FileStream(filePath,FileMode.Create))
                image.CopyTo(stream);
            return fileName;
        }
        private void DeleteImage(string imageName, string section)
        {
            if (string.IsNullOrEmpty(imageName)) return;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/images/{section}", imageName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.productRepository.GetAllProduct().ToList();
            return Json(new { data = products });
        }
    }
}
