using Bulky.Data.Models;
using Bulky.infrastructure.DataBase;
using Bulky.infrastructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository
{
    public class ShoppingCartRepository:Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public IEnumerable<ShoppingCart> GetCart(string UserId)
        {
            return _context.shoppingCarts.Where(x=>x.ApplicationUserId== UserId).Include(x=>x.product).AsEnumerable();
        }
        public int GetCountCart(string UserId)
        {
            return _context.shoppingCarts.Where(x=>x.ApplicationUserId==UserId).Sum(x=>x.Count);
        }
    }
}
