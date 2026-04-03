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
    public class OrderDetailRepository:Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public IEnumerable<OrderDetail> getAll(int orderHeaderId)
        {
            return _context.OrderDetails.Where(x => x.OrderHeaderId == orderHeaderId).Include(x => x.Product).AsNoTracking().AsEnumerable();
        }
    }
}
