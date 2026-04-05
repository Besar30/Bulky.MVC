using Bulky.Data.Mapping;
using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.Repository;
using Bulky.infrastructure.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController(IUnitOfWork unitOfWork) : Controller
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index(string? searsh)
        {
            List<Product> products = _unitOfWork.productRepository.GetAllProduct(searsh).ToList();
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
                productVM.product = _unitOfWork.productRepository.GetProductById(id);
                productVM.ImagesProduct=_unitOfWork.ImageProductRepository.GetImageProudct(id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productvm)
        {
            if (productvm.product.Id == 0 && productvm.Images == null)
            {
                ModelState.AddModelError("product.productImages", "Please upload an image.");
            }
            if (ModelState.IsValid)
            {
                if (productvm.product.Id == 0)
                {
                    _unitOfWork.productRepository.Add(productvm.product);
                    TempData["success"] = "Product created successfully.";
                    _unitOfWork.save();

                    if (productvm.Images != null)
                    {
                        SaveProductImages(productvm.Images, productvm.product.Id);
                        _unitOfWork.save();
                    }
                }
                else
                {
                    Product product = _unitOfWork.productRepository.Get(x => x.Id == productvm.product.Id);
                    productvm.ToEntity(product);
                    if (productvm.Images != null)
                    {
                        SaveProductImages(productvm.Images,product.Id);
                    }
                    _unitOfWork.save();
                    TempData["success"] = "Product updated successfully.";
                }
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
            if (productvm.product.Id != 0)
            {
                vm.ImagesProduct = _unitOfWork.ImageProductRepository.GetImageProudct(productvm.product.Id);
            }
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
            List<ImageProductVM> imageProductVMs = _unitOfWork.ImageProductRepository.GetImageProudct(id);
            foreach(var item in imageProductVMs)
            {
                DeleteImage(item.ImageUrl, $"Product-{id}");
            }
            _unitOfWork.productRepository.Remove(pro);
            _unitOfWork.save();
            TempData["success"] = "Product deleted successflly.";
            return RedirectToAction("Index");
        }
        public IActionResult Deleteimage(int imageId,int productId)
        {
            ProductImage productImage = _unitOfWork.ImageProductRepository.Get(x => x.Id == imageId);
            if (productImage == null) {
                return RedirectToAction(nameof(Upsert), new { id = productId });
            }
            DeleteImage(productImage.ImageUrl, $"Product-{productId}");
            _unitOfWork.ImageProductRepository.Remove(productImage);
            _unitOfWork.save();
            return RedirectToAction(nameof(Upsert), new { id = productId });
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
        private void SaveProductImages(IEnumerable<IFormFile> images, int productId)
        {
            foreach (var img in images)
            {
                string imagePath = SaveImage(img, $"Product-{productId}");
                ProductImage productImage = new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = imagePath
                };
                _unitOfWork.ImageProductRepository.Add(productImage);
            }
            _unitOfWork.save();
        }
        
    }
}
