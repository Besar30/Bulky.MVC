using Bulky.infrastructure.DataBase;
using Bulky.infrastructure.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository categoryRepository { get; private set; }
        private readonly ApplicationDbContext _context;
        public IProductRepository productRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context= context;
            categoryRepository = new CategoryRepository(context);
            productRepository = new ProductRepository(context);
        }
        public void save()
        {
           _context.SaveChanges();
        }
    }
}
