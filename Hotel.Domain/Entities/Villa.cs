using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Display(Name="Price per nigth")]
        [Range(10,10000)]
        public decimal Price { get; set; }
        public int Sqft { get; set; }
        [Range(1,10)]
        public int Occupancy { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; } = "https://placehold.co/600x400";
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set;}
        
        [ValidateNever]
        public IEnumerable<Amenity> VillaAmenity { get; set; }
    }
}
