using System.ComponentModel.DataAnnotations;

namespace Hotel.Web.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        [DataType(DataType.Url)]
        public string? RedirectUrl { get; set; }
    }
}
