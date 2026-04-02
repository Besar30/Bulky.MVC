using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.Migrations;
using Bulky.infrastructure.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppinCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<ShoppingCart> shoppings = _unitOfWork.ShoppingCartRepository.GetCart(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
             ShoppingCartVM = new ShoppinCartVM()
            {
                ListCart = shoppings,
                OrderHeader=new OrderHeader()
            };
            foreach(var cart in ShoppingCartVM.ListCart)
            {
                 cart.Price=GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTolal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            IEnumerable<ShoppingCart> shoppings = _unitOfWork.ShoppingCartRepository.GetCart(UserId);
            ShoppingCartVM = new ShoppinCartVM()
            {
                ListCart = shoppings,
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.applicationUser = _unitOfWork.ApplicationUserRepository.Get(x => x.Id == UserId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.applicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.applicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.applicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.applicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.applicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.applicationUser.PostalCode;
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTolal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        public IActionResult SummaryPost(ShoppinCartVM shoppinCartVM)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            IEnumerable<ShoppingCart> shoppings = _unitOfWork.ShoppingCartRepository.GetCart(UserId);
            shoppinCartVM.ListCart= shoppings;
            shoppinCartVM.OrderHeader.ApplicationUserId= UserId;
            ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.Get(x => x.Id == UserId);
            shoppinCartVM.OrderHeader.OrderDate=DateTime.Now;
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                shoppinCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                shoppinCartVM.OrderHeader.PaymentStatus= SD.StatusPending;
            }
            else
            {
                shoppinCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppinCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeaderRepository.Add(shoppinCartVM.OrderHeader);
            _unitOfWork.save();
            foreach(var item in shoppinCartVM.ListCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = shoppinCartVM.OrderHeader.Id,
                    ProductId = item.product.Id,
                    Count = item.Count,
                    Price = item.product.Price
                };
                _unitOfWork.OrderDetailRepository.Add(orderDetail);
            }
            _unitOfWork.save();
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var Domain = "https://localhost:7014/";
                //strip logic
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = Domain+ $"Customer/Cart/OrderConfirmation/{shoppinCartVM.OrderHeader.Id}",
                    CancelUrl= Domain+"Customer/Cart/Index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach(var item in shoppinCartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Title
                            }

                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                    
                }
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymenrID(shoppinCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation),new {id=shoppinCartVM.OrderHeader.Id});
        }
        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader=_unitOfWork.OrderHeaderRepository.GetById(id);
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymenrID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(id, SD.StatusApproved,SD.PaymentStatusApproved);
                    _unitOfWork.save();
                }
            }
            List<ShoppingCart> shoppings = _unitOfWork.ShoppingCartRepository.GetCart(orderHeader.applicationUser.Id).ToList();
            _unitOfWork.ShoppingCartRepository.RemoveRange(shoppings);
            _unitOfWork.save();
            return View(id);
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
