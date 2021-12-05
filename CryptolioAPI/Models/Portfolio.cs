using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptolioAPI.Models
{
    [Table("portfolios")]
    public class Portfolio
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; }
        [Column("portfolio_name")]
        public string PortfolioName { get; set; }
    }
}