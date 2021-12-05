using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioAddRecord
    {
        [Required] public int PortfolioId { get; set; }
        [Required] public int CoinId { get; set; }
        [Required] public int BuyTime { get; set; }
        [Required] public double BuyPrice { get; set; }
        [Required] public double Amount { get; set; }
        [Required] public string Note { get; set; }
    }
}