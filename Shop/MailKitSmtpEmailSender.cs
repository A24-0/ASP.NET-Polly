using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Shop.Configurations;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Cms;

namespace Shop
{
    public class MailKitSmtpEmailSender : IEmailSender, IAsyncDisposable
    {
        private readonly SmtpClient _smtpClient = new();
        private readonly SmtpConfig _smtpConfig;
        private readonly ILogger<MailKitSmtpEmailSender> _logger;

        public MailKitSmtpEmailSender(IOptionsSnapshot<SmtpConfig> options, ILogger<MailKitSmtpEmailSender> logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            _smtpConfig = options.Value;
            _logger = logger;
        }

        private async Task EnsureConnectedAndAuthenticated()
        {
            int maxConnectionAttempts = 3;
            int currentConnectionAttempt = 0;

            while (!_smtpClient.IsConnected)
            {
                try
                {
                    currentConnectionAttempt++;
                    await _smtpClient.ConnectAsync(_smtpConfig.SmtpServer, _smtpConfig.SmtpPort, SecureSocketOptions.None);
                    _logger.LogInformation("Соединение установлено.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Не удалось установить соединение. Попытка {currentConnectionAttempt}/{maxConnectionAttempts}");
                    currentConnectionAttempt++;
                    if (currentConnectionAttempt < maxConnectionAttempts)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(30));
                        continue;
                    }
                    _logger.LogError(ex, $"Не удалось установить соединение. Попытки исчерпаны");
                    throw;
                }
            }

            if (_smtpClient.IsConnected)
            {
                try
                {
                    if (!_smtpClient.IsAuthenticated)
                    {
                        await _smtpClient.AuthenticateAsync(_smtpConfig.Address, _smtpConfig.Password);
                        _logger.LogInformation("Аутентификация пройдена успешно.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Аутентификация не пройдена. Проверьте данные и повторите попытку");
                    throw;
                }
            }
        }

        public async Task SendEmailAsync(string recipient, string subject, string message, CancellationToken token)
        {
            ArgumentException.ThrowIfNullOrEmpty(recipient);
            ArgumentException.ThrowIfNullOrEmpty(subject);
            ArgumentException.ThrowIfNullOrEmpty(message);

            await EnsureConnectedAndAuthenticated();
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_smtpConfig.UserName, _smtpConfig.Address));
            emailMessage.To.Add(new MailboxAddress("", recipient));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = message
            };
            await _smtpClient.SendAsync(emailMessage);

        }

        public async ValueTask DisposeAsync()
        {
            await _smtpClient.DisconnectAsync(true);
            _smtpClient.Dispose();
        }
    }
}
