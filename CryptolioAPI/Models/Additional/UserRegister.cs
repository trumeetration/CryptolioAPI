using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class UserRegister
    {
        [Required]
        [EmailAddress]
        [StringLength(60)]
        public string Email { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Nickname { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])\S{8,25}$", ErrorMessage = "Password length must be from 8 to 15, and at least 1 special symbol")]
        public string Password { get; set; }
    }
}