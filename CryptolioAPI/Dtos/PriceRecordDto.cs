using System;
using CryptolioAPI.Models;

namespace CryptolioAPI.Dtos
{
    public class PriceRecordDto
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Price { get; set; }
        public Coin Coin { get; set; }
    }
}