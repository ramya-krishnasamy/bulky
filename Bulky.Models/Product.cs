using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models {
	public class Product {
		[Key, ValidateNever]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Title { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string ISBN { get; set; }

		[Required]
		public string Author { get; set; }

        [DisplayName("List Price")]
        [Required]
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [DisplayName("Price for 1-50 book")]
		[Required]
		[Range(1,1000)]
		public double Price { get; set; }

        [DisplayName("Price for 50+ books")]
        [Required]
        [Range(1, 1000)]
        public double Price50 { get; set; }

        [DisplayName("Price for 100+ books")]
        [Required]
        [Range(1, 1000)]
        public double Price100 { get; set; }

		public int CategoryId { get; set; }
		[ForeignKey("CategoryId")]
		[ValidateNever]
		public Category Category { get; set; }

		[ValidateNever]
		public List<ProductImage> productImages { get; set; }
    }
}

