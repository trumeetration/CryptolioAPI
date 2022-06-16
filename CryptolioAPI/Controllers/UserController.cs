using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using CryptolioAPI.Dtos;
using CryptolioAPI.Models;
using CryptolioAPI.Models.Additional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static System.Int32;

namespace CryptolioAPI.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
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
        /// Получить данные пользователя
        /// </summary>
        /// /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int userId)
        {   
            var user = await db.Users.SingleOrDefaultAsync(item => item.Id == userId);
            if (user is null)
            {
                return NotFound();
            }

            return user.AsDto();
        }

        /// <summary>
        /// Получить данные текущего пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserSettingsDto>> GetCurrentUser()
        {
            var userId = GetCurrentUserJwt().Id;
            var user = await db.Users.SingleOrDefaultAsync(item => item.Id == userId);
            if (user is null)
            {
                return NotFound();
            }

            return user.AsDtoSettings();
        }

        /// <summary>
        /// Проверить токен авторизации на валидность
        /// </summary>
        /// <returns></returns>
        [HttpPost("verify")]
        [Authorize]
        public async Task<ActionResult<UserSettingsDto>> VerifyToken()
        {
            return await GetCurrentUser();
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="dataRegister"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ApiResponse> Register([FromBody] UserRegister dataRegister)
        {
            if (await db.Users.SingleOrDefaultAsync(item => item.Email == dataRegister.Email) != null)
            {
                throw new ApiException("Email already exists");
            }
            if (await db.Users.SingleOrDefaultAsync(item => item.Nickname == dataRegister.Nickname) != null)
            {
                throw new ApiException("Nickname already exists");
            }

            User user = new User()
            {
                Email = dataRegister.Email,
                Nickname = dataRegister.Nickname,
                Password = dataRegister.Password,
                CreatedOn = DateTime.UtcNow
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var token = GenerateJwtToken(user);
            return new ApiResponse(message: "", result: token);
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="dataAuth"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ApiResponse> Authorize([FromBody] UserAuth dataAuth)
        {
            var user = await db.Users.SingleOrDefaultAsync(user => user.Email == dataAuth.Email && user.Password == dataAuth.Password);
            if (user == null)
            {
                throw new ApiException("Wrong credentials");
            }

            var token = GenerateJwtToken(user);
            //return user.AsDtoSettings();
            return new ApiResponse(message: user.Nickname, result: token);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("user_id", user.Id.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(1440),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private UserJwt GetCurrentUserJwt()
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
