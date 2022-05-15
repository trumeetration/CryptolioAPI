using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioUpdateRecord
    {
        [Required] 
        public int RecordId { get; set; }
        [Required] 
        public string CoinId { get; set; }
        [Required]
        [Timestamp]
        public int TxTime { get; set; }
        [Required]
        [Range(0, 9999999999)]
        public double TxPrice { get; set; }
        [Required]
        [Range(0, 9999999999)]
        public double Amount { get; set; }
        [MaxLength(500)]
        public string Notes { get; set; }

        [Required] public string RecordType { get; set; }
    }
}