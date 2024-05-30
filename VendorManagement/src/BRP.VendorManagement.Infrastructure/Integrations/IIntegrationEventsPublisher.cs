using BRPartners.Common.IntegrationEvents;

namespace BRP.VendorManagement.Infrastructure.Integrations;

internal interface IIntegrationEventsPublisher
{
    void PublishEvent(IntegrationEvent integrationEvent);
}
