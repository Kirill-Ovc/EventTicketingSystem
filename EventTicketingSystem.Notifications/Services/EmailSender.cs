using EventTicketingSystem.Notifications.Exceptions;
using EventTicketingSystem.Notifications.Interfaces;
using EventTicketingSystem.Notifications.Models;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;

namespace EventTicketingSystem.Notifications.Services
{
    internal class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;
        private const int RetryCount = 3;
        private const int RetryDelay = 4;

        public EmailSender(ILogger<EmailSender> logger,
            IOptions<EmailSettings> options)
        {
            _logger = logger;
            _emailSettings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var policy = CreateRetryPolicy();
            await policy.ExecuteAsync(async () => await SendEmailInternal(email, subject, htmlMessage));
        }

        private AsyncRetryPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(RetryDelay, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} encountered an error: {ExceptionMessage}. Waiting {TimeSpan} before next retry.",
                            retryCount, exception.Message, timeSpan);
                    });
        }

        private async Task SendEmailInternal(string email, string subject, string htmlMessage)
        {
            var client = CreateMailJetClient();
            var request = CreateRequest(email, subject, htmlMessage);
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully. MailJet Response: {Response}", response.GetData());
            }
            else
            {
                var errorMessage = $"Failed to send email. StatusCode: {response.StatusCode} \n" + 
                              $"ErrorInfo: {response.GetErrorInfo()} \n" +
                              $"ErrorMessage: {response.GetErrorMessage()} \n" +
                              $"ResponseData: {response.GetData()}";
                _logger.LogError(errorMessage);

                throw new SendMailException(errorMessage);
            }
        }

        private MailjetClient CreateMailJetClient()
        {
            return new MailjetClient(_emailSettings.ApiKey, _emailSettings.ApiSecret);
        }

        private MailjetRequest CreateRequest(string email, string subject, string htmlMessage)
        {
            var senderEmail = _emailSettings.SenderEmail;
            if (!string.IsNullOrEmpty(_emailSettings.TestEmail))
            {
                email = _emailSettings.TestEmail;
            }

            MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.FromEmail, senderEmail)
                .Property(Send.FromName, "EventTicketingSystem")
                .Property(Send.Subject, subject)
                .Property(Send.TextPart, "Notification")
                .Property(Send.HtmlPart, htmlMessage)
                .Property(Send.Recipients, new JArray {
                    new JObject { {"Email", email} }
                });

            return request;
        }
    }
}
