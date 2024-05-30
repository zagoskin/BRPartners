namespace BRP.Mailer.API.Infrastructure.BackgroundServices;

internal interface IOutboxEmailSendingService
{
    Task CheckForAndSendEmailsAsync(CancellationToken cancellationToken = default);
}
