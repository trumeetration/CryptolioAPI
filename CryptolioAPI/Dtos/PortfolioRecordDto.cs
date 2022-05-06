using System;

namespace CryptolioAPI.Dtos
{
    public class PortfolioRecordDto
    {
        public int Id { get; set; }
        public PortfolioDto Portfolio { get; set; }
        public string CoinId { get; set; }
        public DateTime TxTime { get; set; }
        public double TxPrice { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public string RecordType { get; set; }
        public string Status { get; set; }
    }
}