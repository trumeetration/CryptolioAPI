using System;

namespace CryptolioAPI.Models
{
    public class PortfolioRecord
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int CoinId { get; set; }
        public DateTime BuyTime { get; set; }
        public double BuyPrice { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
    }
}