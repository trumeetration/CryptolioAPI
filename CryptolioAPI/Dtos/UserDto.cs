using System;

namespace CryptolioAPI.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}