using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models {
	public class Company {
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

    }
}

