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

        public IEnumerable<Product> GetAllProduct(string? search)
        {
            
            return _context.products.Where(p => search == null || p.Title.ToLower().Contains(search.ToLower().Trim()))
                .Include(p => p.category) // لو عايز تجيب الكاتيجوري
                .Select(p => new Product
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ISBN = p.ISBN,
                    Author = p.Author,
                    ListPrice = p.ListPrice,
                    Price = p.Price,
                    Price50 = p.Price50,
                    Price100 = p.Price100,
                    CategoryId = p.CategoryId,
                    category = p.category,

                    // جلب أول صورة لو موجودة
                    Image = p.productImages
                             .OrderBy(pi => pi.Id)
                             .Select(pi => pi.ImageUrl)
                             .FirstOrDefault()
                })
                .ToList();
        }

        public Product GetProductById(int? id)
        {
            return _context.products.Where(x => x.Id == id).Include(x => x.category).FirstOrDefault();
        }

        public void Update(Product product)
        {
            _context.Update(product);
        }
    }
}
