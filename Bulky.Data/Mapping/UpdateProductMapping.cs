using Bulky.Data.Models;
using Bulky.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Data.Mapping
{
    public static class UpdateProductMapping
    {
        public static void ToEntity(this ProductVM productVM,Product product)
        {
            product.Title=productVM.product.Title;
            product.Description=productVM.product.Description;
            product.ISBN=productVM.product.ISBN;
            product.Author=productVM.product.Author;
            product.ListPrice=productVM.product.ListPrice;
            product.Price=productVM.product.Price;
            product.Price50=productVM.product.Price50;
            product.Price100=productVM.product.Price100;
            product.CategoryId=productVM.product.CategoryId;
        }
    }
}
