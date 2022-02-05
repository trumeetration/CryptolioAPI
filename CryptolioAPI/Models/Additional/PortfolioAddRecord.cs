using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioAddRecord
    {
        [Required] 
        public int PortfolioId { get; set; }
        [Required] 
        public string CoinId { get; set; }
        [Required]
        public int BuyTime { get; set; }
        [Required]
        [Range(0.00000000001,9999999999)]
        public double BuyPrice { get; set; }
        [Required]
        [Range(0.00000000001, 9999999999)]
        public double Amount { get; set; }
        [Required] 
        [MinLength(1)]
        [MaxLength(500)]
        public string Note { get; set; }
    }
}