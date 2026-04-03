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
    public class OrderHeaderRepository:Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }

        public List<OrderHeader> GetAllOrderHeader()
        {
           return _context.OrderHeaders.Include(x=>x.applicationUser).ToList();
        }
        public IEnumerable<OrderHeader> GetOrdersUser(string UserId)
        {
            return _context.OrderHeaders.Where(x=>x.ApplicationUserId==UserId).Include(x=>x.applicationUser).ToList();
        }
        public OrderHeader GetById(int id)
        {
            return _context.OrderHeaders.Where(x => x.Id == id).Include(x => x.applicationUser).FirstOrDefault();
        }

        public void UpdateStatus(int id, string orderstatus, string? paymentStatus = null)
        {
            var orderheader=_context.OrderHeaders.FirstOrDefault(x=>x.Id== id);
            if (orderheader != null) { 
                orderheader.OrderStatus = orderstatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderheader.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymenrID(int id, string sessionId, string paymentIntentId)
        {
            var orderheader = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderheader != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderheader.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderheader.PaymentIntentId = paymentIntentId;
                    orderheader.PaymentDate= DateTime.Now;
                }
            }
          
        }
    }
}
