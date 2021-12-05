using System.Collections.Generic;
using CryptolioAPI.Models;

namespace CryptolioAPI.Dtos
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public string PortfolioName { get; set; }
    }
}