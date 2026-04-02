using Bulky.Data.Models;

namespace Bulky.Data.ViewModel
{
    public class ShoppinCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
