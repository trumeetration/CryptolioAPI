using CryptolioAPI.Dtos;
using CryptolioAPI.Models;

namespace CryptolioAPI
{
    public static class Extensions
    {
        public static UserDto AsDto(this User user)
        {
            return new UserDto()
            {
                Id = user.Id,
                Nickname = user.Nickname,
                CreatedOn = user.CreatedOn
            };
        }

        public static UserSettingsDto AsDtoSettings(this User user)
        {
            return new UserSettingsDto()
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Email = user.Email,
                CreatedOn = user.CreatedOn
            };
        }

        public static CoinDto AsDto(this Coin user)
        {
            return new CoinDto()
            {
                Id = user.Id,
                ShortName = user.ShortName,
                FullName = user.FullName
            };
        }

        public static PortfolioDto AsDto(this Portfolio portfolio)
        {
            return new PortfolioDto()
            {
                Id = portfolio.Id,
                PortfolioName = portfolio.PortfolioName,
                User = portfolio.User.AsDto()
            };
        }

        public static PortfolioRecordDto AsDto(this PortfolioRecord portfolioRecord)
        {
            return new PortfolioRecordDto()
            {
                Id = portfolioRecord.Id,
                Amount = portfolioRecord.Amount,
                BuyPrice = portfolioRecord.BuyPrice,
                BuyTime = portfolioRecord.BuyTime,
                Coin = portfolioRecord.Coin.AsDto(),
                Notes = portfolioRecord.Notes,
                Portfolio = portfolioRecord.Portfolio.AsDto()
            };
        }
    }
}