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
        [Column("tx_time")] public DateTime TxTime { get; set; }
        [Column("tx_price")] public double TxPrice { get; set; }
        [Column("amount")] public double Amount { get; set; }
        [Column("notes")] public string Notes { get; set; }
        [Column("record_type")] public string RecordType { get; set; }
        [Column("status")] public string Status { get; set; }
    }
}