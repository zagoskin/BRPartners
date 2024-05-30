using MediatR;

namespace BRPartners.Shared.IntegrationEvents;
public class IntegrationEvent : INotification
{
    public virtual string Type { get; set; } = nameof(IntegrationEvent);
}
