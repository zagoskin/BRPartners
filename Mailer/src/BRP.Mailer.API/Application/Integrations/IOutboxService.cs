using BRP.Mailer.API.Domain;

namespace BRP.Mailer.API.Application.Integrations;

internal interface IOutboxService
{
    Task QueueEmailAsync(OutboxEmail emailOutboxEntity);
    Task<OutboxEmail?> GetUnprocessedEmailAsync();
}
