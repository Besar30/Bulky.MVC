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

        public ICompanyRepository CompanyRepository { get; private set; }

        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }

        public IOrderDetailRepository OrderDetailRepository { get; private set; }

        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context= context;
            categoryRepository = new CategoryRepository(context);
            productRepository = new ProductRepository(context);
            CompanyRepository=new CompanyRepository(context);
            ShoppingCartRepository = new ShoppingCartRepository(context);
            ApplicationUserRepository = new ApplicationUserRepository(context);
            OrderHeaderRepository = new OrderHeaderRepository(context);
            OrderDetailRepository = new OrderDetailRepository(context);
        }
        public void save()
        {
           _context.SaveChanges();
        }
    }
}
