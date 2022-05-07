using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioRemoveCoin
    {
        [Required] public int PortfolioId { get; set; }
        [Required] public string CoinId { get; set; }
    }
}