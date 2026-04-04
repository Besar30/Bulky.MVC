using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1,100)]
        [Display(Name ="List Price")]
        public double ListPrice { get; set; }
        [Required]
        [Display(Name = "Price for 1-50")]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Price 50+")]
        public double Price50 { get; set; }
        [Required]
        [Display(Name = "Price 100+")]
        public double Price100 { get; set; }
      
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? category { get; set; }
        [ValidateNever]
        public List<ProductImage> productImages { get; set; }
        [NotMapped]
        [ValidateNever]
       public string Image {  get; set; }
    }
}
