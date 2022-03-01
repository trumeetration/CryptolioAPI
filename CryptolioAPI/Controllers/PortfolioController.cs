using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using CryptolioAPI.Dtos;
using CryptolioAPI.Models;
using CryptolioAPI.Models.Additional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Int32;

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
        /// <returns></returns>
        [HttpGet("user_portfolios")]
        [Authorize]
        public async Task<ApiResponse>
            GetUsersPortfolio()
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userID = currentUser.Id;
            var data = db.Portfolios.Where(portfolio => portfolio.UserId == userID).Include(x => x.User)
                .Select(portfolio => portfolio.AsDto())
                .ToListAsync().Result;
            return new ApiResponse("", data);
        }

        /// <summary>
        /// Создать новый портфель
        /// </summary>
        /// <param name="dataCreate"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<ApiResponse>
            CreatePortfolio(
                [FromBody] PortfolioCreate dataCreate)
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userId = currentUser.Id;
            if (db.Portfolios.Include(x => x.User).SingleOrDefaultAsync(portfolio =>
                portfolio.User.Id == userId && portfolio.PortfolioName == dataCreate.Name).Result is not null)
            {
                throw new ApiException("Portfolio with given name already exists");
            }

            var portfolio = new Portfolio()
            {
                UserId = userId,
                PortfolioName = dataCreate.Name
            };
            db.Portfolios.Add(portfolio);
            await db.SaveChangesAsync();

            portfolio = db.Portfolios.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == portfolio.Id).Result;
            return new ApiResponse("", portfolio.AsDto());
        }

        /// <summary>
        /// Удалить портфель
        /// </summary>
        /// <param name="dataDelete"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Authorize]
        public async Task<ApiResponse>
            DeletePortfolio(
                [FromBody] PortfolioDelete dataDelete)
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userId = currentUser.Id;
            var portfolio = db.Portfolios
                .SingleOrDefaultAsync(x => x.Id == dataDelete.PortfolioId && x.UserId == userId).Result;
            if (portfolio is null)
            {
                throw new ApiException("This portfolio does not exist");
            }

            db.Portfolios.Remove(portfolio);
            await db.SaveChangesAsync();
            return new ApiResponse("Portfolio Deleted");
        }

        /// <summary>
        /// Получить записи в определенном портфеле
        /// </summary>
        /// <param name="portfolioId"></param>
        /// <returns></returns>
        [HttpGet("{portfolioId}")]
        [Authorize]
        public async Task<ApiResponse> GetPortfolioInfo(int portfolioId)
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userId = currentUser.Id;
            var portfolio = db.Portfolios.Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Id == portfolioId && x.UserId == userId).Result;
            if (portfolio is null)
            {
                throw new ApiException("This portfolio does not exist");
            }

            var records = db.PortfolioRecords.Include(x => x.Portfolio)
                .Where(x => x.Portfolio == portfolio).ToListAsync().Result;
            return new ApiResponse("", records.Select(x => x.AsDto()).ToList());
        }

        /// <summary>
        /// Добавить запись в портфель
        /// </summary>
        /// <param name="dataAddRecord"></param>
        /// <returns></returns>
        [HttpPost("{portfolioId}/add")]
        [Authorize]
        public async Task<ApiResponse> AddRecordToPortfolio([FromBody] PortfolioAddRecord dataAddRecord)
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userId = currentUser.Id;
            var portfolio = db.Portfolios
                .SingleOrDefaultAsync(x => x.Id == dataAddRecord.PortfolioId && x.UserId == userId).Result;
            if (portfolio is null)
            {
                throw new ApiException("This portfolio does not exist");
            }

            db.PortfolioRecords.Add(new PortfolioRecord()
            {
                Amount = dataAddRecord.Amount,
                BuyPrice = dataAddRecord.BuyPrice,
                BuyTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(dataAddRecord.BuyTime),
                CoinId = dataAddRecord.CoinId,
                Notes = dataAddRecord.Note,
                PortfolioId = dataAddRecord.PortfolioId
            });
            await db.SaveChangesAsync();
            return new ApiResponse("");
        }

        /// <summary>
        /// Удалить запись из портфеля
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpPost("remove_record/{recordId}")]
        [Authorize]
        public async Task<ApiResponse>
            RemoveRecordFromPortfolio(int recordId)
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userId = currentUser.Id;
            var record = db.PortfolioRecords.Include(x => x.Portfolio)
                .SingleOrDefaultAsync(x => x.Id == recordId && x.Portfolio.UserId == userId).Result;
            if (record is null)
            {
                throw new ApiException("Record not found");
            }

            db.PortfolioRecords.Remove(record);
            await db.SaveChangesAsync();
            return new ApiResponse("Record removed");
        }

        private UserJwt GetCurrentUser()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var userClaims = identity.Claims;
                return new UserJwt()
                {
                    Id = Parse(userClaims.SingleOrDefault(x => x.Type == "user_id")?.Value ?? string.Empty)
                };
            }

            return null;
        }
    }
}