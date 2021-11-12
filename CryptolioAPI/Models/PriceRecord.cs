using System;

namespace CryptolioAPI.Models
{
    public class PriceRecord
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Price { get; set; }
        public int CoinId { get; set; }
    }
}