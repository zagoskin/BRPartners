using BRP.Mailer.API.Application.Integrations;
using BRP.Mailer.API.Application.Mails.Commands.SendEmail;
using BRP.Mailer.API.Domain;
using MediatR;
using MongoDB.Driver;

namespace BRP.Mailer.API.Infrastructure.BackgroundServices;

internal sealed class DefaultOutboxEmailSendingService : IOutboxEmailSendingService
{
    private readonly IOutboxService _outboxService;
    private readonly ISender _sender;
    private readonly IMongoCollection<OutboxEmail> _mongoCollection;

    public DefaultOutboxEmailSendingService(IOutboxService outboxService, ISender sender, IMongoCollection<OutboxEmail> mongoCollection)
    {
        _outboxService = outboxService;
        _sender = sender;
        _mongoCollection = mongoCollection;
    }
    public async Task CheckForAndSendEmailsAsync(CancellationToken cancellationToken = default)
    {
        var unsentEmail = await _outboxService.GetUnprocessedEmailAsync();
        if (unsentEmail is null)
        {
            return;
        }

        var result = await _sender.Send(new SendEmailCommand(
            unsentEmail.To,
            unsentEmail.From,
            unsentEmail.Subject,
            unsentEmail.Body,
            unsentEmail.IsHtmlBody));

        if (result.IsError)
        {
            throw new Exception($"Failed to send email: {string.Join(',', result.Errors.ConvertAll(e => e.Description))}");
        }

        await _mongoCollection.UpdateOneAsync(
            Builders<OutboxEmail>.Filter.Eq(e => e.Id, unsentEmail.Id),
            Builders<OutboxEmail>.Update.Set(e => e.DateTimeUtcProcessed, DateTime.UtcNow),
            cancellationToken: cancellationToken);
    }
}
