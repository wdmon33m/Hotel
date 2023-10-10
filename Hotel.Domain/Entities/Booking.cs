using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public int VillaId { get; set; }
        [ForeignKey("VillaId")]
        public Villa? Villa { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        public decimal TotalCost { get; set; }
        public int Nights { get; set; }
        public string? Status { get; set; }

        [Required]
        public DateTime? BookingDate { get; set; }
        [Required]
        [Display(Name = "Check In Date")]
        public DateTime CheckInDate { get; set; }
        [Required]
        [Display(Name = "Check Out Date")]
        public DateTime CheckOutDate { get; set; }

        public bool IsPaymentSuccessfull { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntendId { get; set;}
        public DateTime ActualCheckInDate { get; set; }
        public DateTime ActualCheckOutDate { get; set; }
        public int VillaNumber { get; set; }
    }
}
