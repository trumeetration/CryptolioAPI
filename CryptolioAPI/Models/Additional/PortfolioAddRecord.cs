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
        [Timestamp]
        public int BuyTime { get; set; }
        [Required]
        [Range(0, 9999999999)]
        public double BuyPrice { get; set; }
        [Required]
        [Range(0, 9999999999)]
        public double Amount { get; set; }
        [Required] 
        [MinLength(1)]
        [MaxLength(500)]
        public string Note { get; set; }

        [Required] public string RecordType { get; set; }
    }
}