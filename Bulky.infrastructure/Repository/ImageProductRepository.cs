using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using Bulky.infrastructure.DataBase;
using Bulky.infrastructure.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository
{
    public class ImageProductRepository:Repository<ProductImage>, IImageProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ImageProductRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public List<ImageProductVM> GetImageProudct(int? id)
        {
            return _context.productImages.Where(x => x.ProductId == id).Select(x => new ImageProductVM {ImageUrl=x.ImageUrl,Id=x.Id }).ToList();
        }
    }
}
