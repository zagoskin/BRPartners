using BRP.Mailer.API.Application.Integrations;
using BRP.Mailer.API.Domain;
using BRPartners.Common.IntegrationEvents.VendorManagement;
using MediatR;

namespace BRP.Mailer.API.Application.Integrations.Contracts.Created;

internal sealed class ContractCreatedIntegrationEventHandler : INotificationHandler<ContractCreatedIntegrationEvent>
{
    private readonly IOutboxService _outboxService;

    public ContractCreatedIntegrationEventHandler(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }
    public async Task Handle(ContractCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var emailOutbox = new OutboxEmail(
            to: notification.DoneBy,
            from: "no-reply@brp.com",
            subject: $"[Contract Created] {notification.Title}",
            body: $"Contract '{notification.Title}' with id {notification.ContractId} has been created and is awaiting review.",
            isHtmlBody: false);

        await _outboxService.QueueEmailAsync(emailOutbox);
    }
}
