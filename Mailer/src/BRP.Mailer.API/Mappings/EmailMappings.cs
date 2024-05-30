using BRP.Mailer.API.Domain;
using BRPartners.Shared.Responses.Mailer;

namespace BRP.Mailer.API.Mappings;

internal static class EmailMappings
{
    public static EmailResponse ToResponse(this OutboxEmail email)
    {
        return new EmailResponse
        {
            Id = email.Id,
            To = email.To,
            From = email.From,
            Subject = email.Subject,
            Body = email.Body,
            IsHtmlBody = email.IsHtmlBody,
            DateTimeUtcProcessed = email.DateTimeUtcProcessed
        };
    }
}
