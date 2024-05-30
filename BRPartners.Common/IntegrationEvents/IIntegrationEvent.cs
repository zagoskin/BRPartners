using MediatR;

namespace BRPartners.Common.IntegrationEvents;
public class IntegrationEvent : INotification
{
    public virtual string Type { get; set; } = nameof(IntegrationEvent);
}
