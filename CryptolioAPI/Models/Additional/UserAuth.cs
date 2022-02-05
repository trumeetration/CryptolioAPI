using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class UserAuth
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(40)]
        public string Password { get; set; }
    }
}