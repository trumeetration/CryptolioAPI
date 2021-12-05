using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptolioAPI.Models
{
    [Table("coins")]
    public class Coin
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("short_name")]
        public string ShortName { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
    }
}