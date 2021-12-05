using System.Collections.Generic;
using System.Linq;
using CryptolioAPI.Dtos;
using CryptolioAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptolioAPI.Controllers
{
    [ApiController]
    [Route("coins")]
    public class CoinController : ControllerBase
    {
        private ApplicationContext db;
        public CoinController(ApplicationContext context)
        {
            db = context;
        }

        /// <summary>
        /// Показать существующие монеты
        /// </summary>
        [HttpGet("all")]
        public ActionResult<IEnumerable<CoinDto>> GetCoinList()
        {
            return db.Coins.Select(coin => coin.AsDto()).ToList();
        }
    }
}