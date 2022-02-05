using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptolioAPI.Models
{
    [Table("portfolio_records")]
    public class PortfolioRecord
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("portfolio_id")] public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; }
        [Column("coin_id")] public string CoinId { get; set; }
        [Column("buy_time")] public DateTime BuyTime { get; set; }
        [Column("buy_price")] public double BuyPrice { get; set; }
        [Column("amount")] public double Amount { get; set; }
        [Column("notes")] public string Notes { get; set; }
    }
}