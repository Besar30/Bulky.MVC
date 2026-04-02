using Bulky.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
        IEnumerable<Product> GetAllProduct();
        void Update(Product product);
    }
}
