using Microsoft.Extensions.Hosting.Internal;

namespace Shop
{
    public class AppRunNotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<AppRunNotificationBackgroundService> _logger;
        public AppRunNotificationBackgroundService(ILogger<AppRunNotificationBackgroundService> logger,
            ApplicationLifetime applicationLifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogInformation("Приложение запущено");
            });
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервер запущен");
        }
    }
}
