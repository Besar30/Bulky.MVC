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
    public class ProductRepository:Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProduct()
        {
            return _context.products.Include(x=>x.category).ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.products.Where(x => x.Id == id).Include(x => x.category).FirstOrDefault();
        }

        public void Update(Product product)
        {
            _context.Update(product);
        }
    }
}
