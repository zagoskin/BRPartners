using System.Text.Json;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.Events;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using BRP.VendorManagement.Infrastructure.Persistence;
using BRP.VendorManagement.Infrastructure.Persistence.OutboxIntegrationEvents;
using BRPartners.Common.IntegrationEvents;
using BRPartners.Common.IntegrationEvents.VendorManagement;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BRP.VendorManagement.Infrastructure.Integrations;
internal sealed class OutboxDomainToIntegrationEventHandler :
    INotificationHandler<ContractCreatedDomainEvent>
{
    private readonly VendorManagementDbContext _context;
    private readonly ILogger<OutboxDomainToIntegrationEventHandler> _logger;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public OutboxDomainToIntegrationEventHandler(VendorManagementDbContext context, ILogger<OutboxDomainToIntegrationEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ContractCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var contract = await GetFullContractOrThrowAsync(notification.ContractId, cancellationToken);

        var integrationEvent = new ContractCreatedIntegrationEvent
        {
            ContractId = contract.Id.Value.ToString(),
            Title = contract.Title,
            VendorId = contract.VendorId.Value.ToString(),
            DeadLineUtc = contract.DeadLineUtc,
            EstimatedValue = contract.EstimatedValue,
            DoneBy = notification.DoneBy
        };

        await AddOutboxIntegrationEventAsync(integrationEvent);
    }    

    private async Task<Contract> GetFullContractOrThrowAsync(Guid contractId, CancellationToken cancellationToken)
    {
        var contractStrongTypedId = ContractId.Create(contractId);
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.Id == contractStrongTypedId, cancellationToken: cancellationToken);

        if (contract is null)
        {
            _logger.LogWarning("Contract with id {ContractId} not found", contractId);
            throw new InvalidOperationException($"Contract with id {contractId} not found");
        }

        return contract;
    }

    private async Task AddOutboxIntegrationEventAsync(IntegrationEvent integrationEvent)
    {
        var type = integrationEvent.GetType();
        await _context.OutboxIntegrationEvents.AddAsync(new OutboxIntegrationEvent(
            type: type.Name,
            content: JsonSerializer.Serialize(
                integrationEvent,
                type,
                _jsonSerializerOptions)));
        await _context.SaveChangesAsync();
    }
}
