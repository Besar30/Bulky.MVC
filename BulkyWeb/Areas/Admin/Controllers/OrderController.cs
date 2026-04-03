using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string? status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAllOrderHeader();
            }
            else
            {
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetOrdersUser(UserId);
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return View(orderHeaders);
        }

        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new OrderVM
            {
                orderHeader = _unitOfWork.OrderHeaderRepository.GetById(orderId),
                orderDetails = _unitOfWork.OrderDetailRepository.getAll(orderId)
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Customer)]
        public IActionResult UpdateOrderDetails(OrderVM orderVM)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.Get(x => x.Id == orderVM.orderHeader.Id);
            orderHeader.Name = orderVM.orderHeader.Name;
            orderHeader.PhoneNumber = orderVM.orderHeader.PhoneNumber;
            orderHeader.StreetAddress = orderVM.orderHeader.StreetAddress;
            orderHeader.City = orderVM.orderHeader.City;
            orderHeader.State = orderVM.orderHeader.State;
            orderHeader.PostalCode = orderVM.orderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderVM.orderHeader.Carrier))
            {
                orderHeader.Carrier = orderVM.orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
            {
                orderHeader.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            }
            _unitOfWork.save();
            TempData[key: "success"] = "Order Details Updated successfly";
            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Customer)]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetById(orderVM.orderHeader.Id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.save();
            TempData["success"] = "Order Details Updated successfly";

            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Customer)]
        public IActionResult ShipOrder(OrderVM orderVM)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetById(orderVM.orderHeader.Id);
            orderHeader.Carrier = orderVM.orderHeader.Carrier;
            orderHeader.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            orderHeader.ShippingDate = orderVM.orderHeader.ShippingDate;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            orderHeader.OrderStatus = SD.StatusShipped;
            _unitOfWork.save();
            TempData["success"] = "Order Details Updated successfly";

            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Customer)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetById(orderVM.orderHeader.Id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusRefunded;
             }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusCancelled;
            }
            _unitOfWork.save();
            TempData["success"] = "Order Canceled successfly";

            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });

        }
        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_Pay_Now(OrderVM orderVM) {
            orderVM.orderHeader = _unitOfWork.OrderHeaderRepository.GetById(orderVM.orderHeader.Id);
            orderVM.orderDetails=_unitOfWork.OrderDetailRepository.getAll(orderVM.orderHeader.Id);
            var Domain = "https://localhost:7014/";
            //strip logic
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = Domain + $"Admin/Order/PaymentConfirmation/{orderVM.orderHeader.Id}",
                CancelUrl = Domain + $"Admin/Order/Details/{orderVM.orderHeader.Id}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var item in orderVM.orderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }

                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);

            }
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            _unitOfWork.OrderHeaderRepository.UpdateStripePaymenrID(orderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult PaymentConfirmation(int Id)
        {
            OrderHeader orderHeader=_unitOfWork.OrderHeaderRepository.GetById(Id);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymenrID(Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(Id, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.save();
                }
            }
            return View(Id);
        }
    }
}
