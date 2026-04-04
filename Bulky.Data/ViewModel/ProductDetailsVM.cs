
using Bulky.Data.Models;

namespace Bulky.Data.ViewModel
{
    public class ProductDetailsVM
    {
       public List<ImageProductVM> ProductImage { get; set; }
       public ShoppingCart shoppingCart { get; set; }
    }
}
