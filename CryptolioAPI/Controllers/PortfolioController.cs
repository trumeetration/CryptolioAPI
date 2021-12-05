using System;
using System.Collections.Generic;
using System.Linq;
using CryptolioAPI.Dtos;
using CryptolioAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CryptolioAPI.Controllers
{
    [ApiController]
    [Route("portfolio")]
    public class PortfolioController : ControllerBase
    {
        private ApplicationContext db;

        public PortfolioController(ApplicationContext context)
        {
            db = context;
        }

        /// <summary>
        /// Получить данные о портфелях пользователя
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet("user_portfolios")]
        public ActionResult<IEnumerable<PortfolioDto>>
            GetUsersPortfolio(int user_id) //user_id - temporary solution until jwt will be included
        {
            return db.Portfolios.Where(portfolio => portfolio.UserId == user_id).Include(x => x.User).Select(portfolio => portfolio.AsDto())
                .ToList();
        }

        /// <summary>
        /// Создать новый портфель
        /// </summary>
        /// <param name="name"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public ActionResult<PortfolioDto>
            CreatePortfolio(string name, int user_id) //user_id - temporary solution until jwt will be included
        {
            if (db.Portfolios.Include(x => x.User).SingleOrDefault(portfolio =>
                portfolio.User.Id == user_id && portfolio.PortfolioName == name) is not null)
            {
                return Conflict("Portfolio with given name already exists");
            }

            var portfolio = new Portfolio()
            {
                UserId = user_id,
                PortfolioName = name
            };
            db.Portfolios.Add(portfolio);
            db.SaveChanges();

            portfolio = db.Portfolios.Include(x => x.User).SingleOrDefault(x => x.Id == portfolio.Id);
            return portfolio.AsDto();
        }

        /// <summary>
        /// Удалить портфель
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        public ActionResult
            DeletePortfolio(int portfolio_id, int user_id) ////user_id - temporary solution until jwt will be included
        {
            var portfolio = db.Portfolios.SingleOrDefault(x => x.Id == portfolio_id && x.UserId == user_id);
            if (portfolio is null)
            {
                return Conflict("Error with deleting portfolio");
            }

            db.Portfolios.Remove(portfolio);
            return Ok("Portfolio Deleted");
        }

        /// <summary>
        /// Получить записи в определенном портфеле
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <returns></returns>
        [HttpGet("{portfolio_id}")]
        public ActionResult<IEnumerable<PortfolioRecordDto>> GetPortfolioInfo(int portfolio_id)
        {
            var portfolio = db.Portfolios.Include(x => x.User).SingleOrDefault(x => x.Id == portfolio_id);
            if (portfolio is null)
            {
                return Conflict("Error with getting portfolio");
            }

            var records = db.PortfolioRecords.Include(x => x.Portfolio).Include(x => x.Coin)
                .Where(x => x.Portfolio == portfolio).ToList();
            return records.Select(x => x.AsDto()).ToList();
        }

        /// <summary>
        /// Добавить запись в портфель
        /// </summary>
        /// <param name="portfolio_id"></param>
        /// <param name="coin_id"></param>
        /// <param name="buytime"></param>
        /// <param name="buyprice"></param>
        /// <param name="amount"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost("{portfolio_id}/add")]
        public ActionResult AddRecordToPortfolio(int portfolio_id, int coin_id, int buytime, int buyprice, int amount,
            string note = "")
        {
            var portfolio = db.Portfolios.FirstOrDefault(x => x.Id == portfolio_id);
            if (portfolio is null)
            {
                return Conflict("Error with getting portfolio");
            }

            db.PortfolioRecords.Add(new PortfolioRecord()
            {
                Amount = amount,
                BuyPrice = buyprice,
                BuyTime = new DateTime(1970,1,1,0,0,0,0).AddSeconds(buytime),
                CoinId = coin_id,
                Notes = note,
                PortfolioId = portfolio_id
            });
            db.SaveChanges();
            return Ok("Record added");
        }

        /// <summary>
        /// Удалить запись из портфеля
        /// </summary>
        /// <param name="record_id"></param>
        /// <returns></returns>
        [HttpPost("remove_record/{record_id}")]
        public ActionResult RemoveRecordFromPortfolio(int record_id) // check next if record not assigned with authorized user
        {
            var record = db.PortfolioRecords.SingleOrDefault(x => x.Id == record_id);
            if (record is null)
            {
                return Conflict("Error with removing record");
            }

            db.PortfolioRecords.Remove(record);
            return Ok("Record removed");
        }
    }
}