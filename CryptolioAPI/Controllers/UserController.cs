using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CryptolioAPI.Dtos;
using CryptolioAPI.Models;
using CryptolioAPI.Models.Additional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static System.Int32;

namespace CryptolioAPI.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private ApplicationContext db;
        private IConfiguration _config;
        public UserController(ApplicationContext context, IConfiguration configuration)
        {
            db = context;
            _config = configuration;
        }

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            return db.Users.Select(item => item.AsDto()).ToList();
        }

        /// <summary>
        /// Получить данные пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<UserDto> GetUser()
        {
            var currentUser = GetCurrentUser();
            if (currentUser is null)
            {
                return Conflict("Cant resolve user from jwt");
            }
            var user = db.Users.SingleOrDefault(item => item.Id == currentUser.Id);
            if (user is null)
            {
                return NotFound();
            }

            return user.AsDto();
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="email"></param>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult<UserSettingsDto> Register(string email, string nickname, string password)
        {
            if (db.Users.SingleOrDefault(item => item.Email == email) != null)
            {
                return Conflict("Email already exists");
            }
            if (db.Users.SingleOrDefault(item => item.Nickname == nickname) != null)
            {
                return Conflict("Nickname already exists");
            }

            User user = new User()
            {
                Email = email,
                Nickname = nickname,
                Password = password,
                CreatedOn = DateTime.UtcNow
            };
            db.Users.Add(user);
            db.SaveChanges();
            return user.AsDtoSettings();
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<UserSettingsDto> Authorize(string email, string password)
        {
            var user = db.Users.SingleOrDefault(user => user.Email == email && user.Password == password);
            if (user == null)
            {
                return Conflict("Wrong Credentials");
            }

            var token = GenerateJwtToken(user);
            //return user.AsDtoSettings();
            return Ok(token);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("user_id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("nickname", user.Nickname)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(1440),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserJwt GetCurrentUser()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var userClaims = identity.Claims;
                return new UserJwt()
                {
                    Email = userClaims.SingleOrDefault(x => x.Type == "email")?.Value,
                    Id = Parse(userClaims.SingleOrDefault(x => x.Type == "user_id")?.Value),
                    Nickname = userClaims.SingleOrDefault(x => x.Type == "nickname")?.Value
                };
            }

            return null;
        }
    }
}
