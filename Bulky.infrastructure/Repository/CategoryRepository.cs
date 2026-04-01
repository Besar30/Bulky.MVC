using Bulky.Data.Models;
using Bulky.infrastructure.DataBase;
using Bulky.infrastructure.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository
{
    public class CategoryRepository:Repository<Category>,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(Category category)
        {
            _context.Update(category);
        }
    }
}
