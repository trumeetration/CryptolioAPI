using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioCreate
    {
        [Required]
        [MinLength(1)]
        [MaxLength(40)]
        public string Name { get; set; }
    }
}