using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models {
	public class OrderHeader {
		[Key]
		public int Id { get; set; }

		public string UserId { get; set; }
		[ForeignKey("UserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

		public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }

        public string? SessionId { get; set; }
		public string? PaymentIntendId { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

