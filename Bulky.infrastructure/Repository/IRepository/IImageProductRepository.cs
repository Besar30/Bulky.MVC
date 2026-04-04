using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.infrastructure.Repository.IRepository
{
    public interface IImageProductRepository:IRepository<ProductImage>
    {
        List<ImageProductVM> GetImageProudct(int? id);
    }
}
