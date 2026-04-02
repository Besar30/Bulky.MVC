using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<ShoppingCart> shoppings = _unitOfWork.ShoppingCartRepository.GetCart(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ShoppinCartVM shoppinCartVM = new ShoppinCartVM()
            {
                ListCart = shoppings,
            };
            foreach(var cart in shoppinCartVM.ListCart)
            {
                 cart.Price=GetPriceBasedOnQuantity(cart);
                shoppinCartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shoppinCartVM);
        }
        public IActionResult Summary()
        {
            return View();
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.product.Price;
            }
            else if(shoppingCart.Count <= 100)
            {
                return shoppingCart.product.Price50;
            }
            else
            {
                return shoppingCart.product.Price100;
            }
        }
    }
}
