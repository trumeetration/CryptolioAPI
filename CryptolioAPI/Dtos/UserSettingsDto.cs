using System;

namespace CryptolioAPI.Dtos
{
    public class UserSettingsDto
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}