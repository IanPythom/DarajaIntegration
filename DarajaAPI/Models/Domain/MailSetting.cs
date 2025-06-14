﻿using System.ComponentModel.DataAnnotations;

namespace DarajaAPI.Models.Domain
{
    public class MailSetting
    {
        [Key]
        public Guid Id { get; set; }
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
