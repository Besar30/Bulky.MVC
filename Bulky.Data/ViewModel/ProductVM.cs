using Bulky.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Data.ViewModel
{
    public class ProductVM
    {
        public Product product { get; set; }
        public List<IFormFile>? Images {  get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        [ValidateNever]
        public List<ImageProductVM>? ImagesProduct {  get; set; }
    }
}
