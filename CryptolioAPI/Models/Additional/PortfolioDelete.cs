using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioDelete
    {
        [Required]
        public int PortfolioId { get; set; }
    }
}