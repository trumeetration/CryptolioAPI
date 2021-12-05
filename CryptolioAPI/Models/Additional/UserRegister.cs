using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class UserRegister
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Nickname { get; set; }
        [Required]
        public string Password { get; set; }
    }
}