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
            var userId = GetCurrentUser().Id;
            var data = await db.Portfolios.Where(portfolio => portfolio.UserId == userId).Include(x => x.User)
                .Select(portfolio => portfolio.AsDto())
                .ToListAsync();
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
            var userId = GetCurrentUser().Id;
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
        /// Редактировать название портфеля
        /// </summary>
        /// <param name="dataUpdate"></param>
        /// <returns></returns>
        [HttpPost("update")]
        [Authorize]
        public async Task<ApiResponse>
            UpdatePortfolio(
                [FromBody] PortfolioUpdate dataUpdate)
        {
            var userId = GetCurrentUser().Id;
            var portfolio = await db.Portfolios.Include(x => x.User).SingleOrDefaultAsync(portfolio1 =>
                portfolio1.User.Id == userId && dataUpdate.Id == portfolio1.Id);
            if (portfolio is null)
            {
                throw new ApiException("Wrong data given");
            }

            portfolio.PortfolioName = dataUpdate.Name;
            await db.SaveChangesAsync();
            return new ApiResponse("Updated", portfolio.AsDto());
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
            var userId = GetCurrentUser().Id;
            var portfolio = await db.Portfolios
                .SingleOrDefaultAsync(x => x.Id == dataDelete.PortfolioId && x.UserId == userId);
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
        public async Task<ApiResponse>
            GetPortfolioInfo(
                int portfolioId)
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                throw new ApiException("Wrong auth data");
            }

            var userId = currentUser.Id;
            var portfolio = await db.Portfolios.Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Id == portfolioId && x.UserId == userId);
            if (portfolio is null)
            {
                throw new ApiException("This portfolio does not exist");
            }

            var records = await db.PortfolioRecords.Include(x => x.Portfolio)
                .Where(x => x.Portfolio == portfolio)
                .ToListAsync();
            return new ApiResponse("", records.Select(x => x.AsDto()).ToList());
        }

        /// <summary>
        /// Добавить запись в портфель
        /// </summary>
        /// <param name="dataAddRecord"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize]
        public async Task<ApiResponse>
            AddRecordToPortfolio(
                [FromBody] PortfolioAddRecord dataAddRecord)
        {
            var userId = GetCurrentUser().Id;
            var portfolio = await db.Portfolios
                .SingleOrDefaultAsync(x => x.Id == dataAddRecord.PortfolioId && x.UserId == userId);
            if (portfolio is null)
            {
                throw new ApiException("This portfolio does not exist");
            }

            var currentTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (dataAddRecord.TxTime > currentTimestamp)
            {
                throw new ApiException("Wrong buytime was specified");
            }

            if (dataAddRecord.RecordType is not "buy" and not "sell" and not "follow")
            {
                throw new ApiException("Wrong record type was specified");
            }

            db.PortfolioRecords.Add(new PortfolioRecord()
            {
                Amount = dataAddRecord.Amount,
                TxPrice = dataAddRecord.TxPrice,
                TxTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(dataAddRecord.TxTime),
                CoinId = dataAddRecord.CoinId,
                Notes = dataAddRecord.Notes,
                RecordType = dataAddRecord.RecordType,
                PortfolioId = dataAddRecord.PortfolioId,
                Status = "live"
            });
            await db.SaveChangesAsync();
            return new ApiResponse("Record added");
        }

        /// <summary>
        /// Обновить существующую запись в портфеле
        /// </summary>
        /// <param name="dataUpdateRecord"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        [HttpPost("update_record")]
        [Authorize]
        public async Task<ApiResponse>
            UpdateRecordInfo(
                [FromBody] PortfolioUpdateRecord dataUpdateRecord)
        {
            var userId = GetCurrentUser().Id;
            var record = await db.PortfolioRecords.Include(x => x.Portfolio)
                .SingleOrDefaultAsync(x => x.Portfolio.UserId == userId && x.Id == dataUpdateRecord.RecordId);
            if (record is null)
            {
                throw new ApiException("Record not found");
            }
            
            var currentTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (dataUpdateRecord.TxTime > currentTimestamp)
            {
                throw new ApiException("Wrong buytime was specified");
            }

            if (dataUpdateRecord.RecordType is not "buy" and not "sell" and not "follow")
            {
                throw new ApiException("Wrong record type was specified");
            }
            
            record.Amount = dataUpdateRecord.Amount;
            record.Notes = dataUpdateRecord.Notes;
            record.CoinId = dataUpdateRecord.CoinId;
            record.RecordType = dataUpdateRecord.RecordType;
            record.TxPrice = dataUpdateRecord.TxPrice;
            record.TxTime = Convert.ToDateTime(dataUpdateRecord.TxTime);

            await db.SaveChangesAsync();

            return new ApiResponse("Record updated");
        }
        
        /// <summary>
        /// Поместить в корзину или удалить запись из портфеля
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpPost("remove_record/{recordId}")]
        [Authorize]
        public async Task<ApiResponse>
            RemoveRecordFromPortfolio(int recordId)
        {
            var userId = GetCurrentUser().Id;
            var record = await db.PortfolioRecords.Include(x => x.Portfolio)
                .SingleOrDefaultAsync(x => x.Id == recordId && x.Portfolio.UserId == userId);
            if (record is null)
            {
                throw new ApiException("Record not found");
            }
            
            switch (record.Status)
            {
                case "live":
                    if (record.RecordType == "follow")
                        db.PortfolioRecords.Remove(record);
                    else 
                        record.Status = "trash";
                    break;
                case "trash":
                    db.PortfolioRecords.Remove(record);
                    break;
            }

            await db.SaveChangesAsync();
            return new ApiResponse("Record removed");
        }

        /// <summary>
        /// Удалить все записи по определенной монете в портфеле
        /// </summary>
        /// <returns></returns>
        [HttpPost("remove_coin")]
        [Authorize]
        public async Task<ApiResponse> RemoveCoinRecordsFromPortfolio([FromBody] PortfolioRemoveCoin data)
        {
            var userId = GetCurrentUser().Id;
            var records = await db.PortfolioRecords.Include(x => x.Portfolio)
                .Where(x => x.Portfolio.UserId == userId
                            && x.PortfolioId == data.PortfolioId
                            && x.CoinId == data.CoinId
                            && x.Status == "live")
                .ToListAsync();
            if (records.Count == 0)
            {
                throw new ApiException("No records found with given data");
            }

            foreach (var record in records)
            {
                if (record.RecordType == "follow")
                    db.PortfolioRecords.Remove(record);
                else
                    record.Status = "trash";
            }
            await db.SaveChangesAsync();
            return new ApiResponse("Coin transactions were moved to trash bin");
        }

        /// <summary>
        /// Восстановить транзакцию из корзины
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("recover")]
        [Authorize]
        public async Task<ApiResponse> RecoverTransactionFromTrashbin([FromBody] PortfolioRecoverTx data)
        {
            var userId = GetCurrentUser().Id;
            var record = await db.PortfolioRecords
                .Include(x => x.Portfolio)
                .SingleOrDefaultAsync(x => x.Portfolio.UserId == userId
                                           && x.Status == "trash"
                                           && x.Id == data.RecordId);
            if (record is null)
            {
                throw new ApiException("No record found");
            }

            record.Status = "live";
            await db.SaveChangesAsync();
            return new ApiResponse("Transaction removed from trash");
        }
        private UserJwt GetCurrentUser()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity) return null;
            var userClaims = identity.Claims;
            return new UserJwt()
            {
                Id = Parse(userClaims.SingleOrDefault(x => x.Type == "user_id")?.Value ?? string.Empty)
            };
        }
    }
}