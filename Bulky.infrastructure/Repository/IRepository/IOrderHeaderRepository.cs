using Bulky.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void UpdateStatus(int id,string orderstatus,string? paymentStatus=null);
        void UpdateStripePaymenrID(int id, string sessionId, string paymentIntentId);
        public IEnumerable<OrderHeader> GetOrdersUser(string UserId);

        OrderHeader GetById(int id);
        List<OrderHeader> GetAllOrderHeader();
    }
}
