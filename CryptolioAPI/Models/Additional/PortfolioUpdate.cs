using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioUpdate
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [MinLength(1)]
        [MaxLength(40)]
        public string Name { get; set; }
    }
}