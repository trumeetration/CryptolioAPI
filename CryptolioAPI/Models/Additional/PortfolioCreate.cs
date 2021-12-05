using System.ComponentModel.DataAnnotations;

namespace CryptolioAPI.Models.Additional
{
    public class PortfolioCreate
    {
        [Required]
        public string Name { get; set; }
    }
}