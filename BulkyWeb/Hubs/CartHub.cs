using Bulky.infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BulkyWeb.Hubs
{
    public class CartHub:Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> GetCartCount()
        {
            string UserId=Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null) { 
                return 0;
            }
            var count=_unitOfWork.ShoppingCartRepository.GetCountCart(UserId);
            return count;
        }
    }
}
