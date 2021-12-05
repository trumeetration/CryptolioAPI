using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class UserAuth
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}