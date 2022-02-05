using System;

namespace CryptolioAPI.Dtos
{
    public class PortfolioRecordDto
    {
        public int Id { get; set; }
        public PortfolioDto Portfolio { get; set; }
        public string CoinId { get; set; }
        public DateTime BuyTime { get; set; }
        public double BuyPrice { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
    }
}