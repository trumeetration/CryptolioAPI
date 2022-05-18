using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioRecoverTx
    {
        [Required]
        public int RecordId { get; set; }
    }
}