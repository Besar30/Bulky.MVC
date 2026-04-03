using Bulky.infrastructure.Repository.IRepository;
using BulkyWeb.Services;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BulkyWeb.Hubs
{
    public class CartHub:Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheServices _cacheServices;
        public CartHub(IUnitOfWork unitOfWork, ICacheServices cacheServices)
        {
            _unitOfWork = unitOfWork;
            _cacheServices = cacheServices;
            
        }
        public async Task<int> GetCartCount()
        {
            string UserId=Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null) { 
                return 0;
            }
            var cacheKey=$"CartKey_{UserId}";
            var cache = await _cacheServices.GetAsync<int>(cacheKey);
            if (cache !=0)
            {
                return cache;
            }
            var count=_unitOfWork.ShoppingCartRepository.GetCountCart(UserId);
            await _cacheServices.SetAsync(cacheKey, count);
            return count;
        }
    }
}
