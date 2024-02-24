using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using Shop.Configurations;

namespace Shop
{
    public class RetrySendDecorator : IEmailSender
    {
        private readonly IEmailSender _innerEmailSender;
        private readonly ILogger<RetrySendDecorator> _logger;
        private readonly SmtpConfig _conf;
        private readonly AsyncRetryPolicy _policy;

        public RetrySendDecorator(IEmailSender innerEmailSender, ILogger<RetrySendDecorator> logger, IOptionsSnapshot<SmtpConfig> options)
        {
            ArgumentNullException.ThrowIfNull(innerEmailSender);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(options);
            _innerEmailSender = innerEmailSender;
            _logger = logger;
            _conf = options.Value;

            _policy = Policy
                .Handle<ConnectionException>()
                .WaitAndRetryAsync(_conf.RetryLimit, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, retryAttempt) =>
                {
                    _logger.LogWarning(exception, "Error while sending email. Retrying: {Attempt}", retryAttempt);
                });
        }

        public async Task SendEmailAsync(string recipient, string subject, string message, CancellationToken token)
        {
            PolicyResult? result = await _policy.ExecuteAndCaptureAsync(
                       () => _innerEmailSender.SendEmailAsync("desu99@bk.ru", "Connecting", "Working", default));

            if (result.Outcome == OutcomeType.Failure) throw result.FinalException;


        }
    }
}