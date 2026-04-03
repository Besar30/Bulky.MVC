using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.Data.Models;
using Bulky.infrastructure.Repository.IRepository;
using BulkyWeb.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products=_unitOfWork.productRepository.GetAll();
            return View(products);
        }
        public IActionResult Details(int id)
        {
            Product product = _unitOfWork.productRepository.GetProductById(id);
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                product = product,
                Count=1,
                ProductId= id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null) {
                return Unauthorized();
            }
            ShoppingCart shopping = _unitOfWork.ShoppingCartRepository.Get(x => x.ApplicationUserId == UserId && x.ProductId == shoppingCart.ProductId);
            if (shopping != null)
            {
                shopping.Count += shoppingCart.Count;
            }
            else
            {
                shoppingCart.Id = 0;
                shoppingCart.ApplicationUserId = UserId;
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
            }
            TempData["success"] = "Cart Updated Successfly";
            _unitOfWork.save();
            int totalCount = _unitOfWork.ShoppingCartRepository
                     .GetCart(UserId)
                     .Sum(c => c.Count);
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
