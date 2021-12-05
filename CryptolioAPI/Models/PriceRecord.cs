using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptolioAPI.Models
{
    [Table("price_records")]
    public class PriceRecord
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("time_stamp")] public DateTime TimeStamp { get; set; }
        [Column("price")] public double Price { get; set; }
        [Column("coin_id")] public int CoinId { get; set; }
        public Coin Coin { get; set; }
    }
}