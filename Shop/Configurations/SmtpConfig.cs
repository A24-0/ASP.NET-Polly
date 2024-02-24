#pragma warning disable CS8618 //отключаем null-ворнинги
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Shop.Configurations
{
    public class SmtpConfig
    {
        [Required] public string SmtpServer { get; set; }
        [Range(1, ushort.MaxValue)] public int SmtpPort { get; set; }
        [EmailAddress] public string Address { get; set; }
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        [Range(1, 1000)]
        public int RetryLimit { get; set; }


    }
}
