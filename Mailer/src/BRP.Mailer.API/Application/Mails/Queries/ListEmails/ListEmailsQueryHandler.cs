using BRP.Mailer.API.Domain;
using BRPartners.Shared.Responses.Mailer;
using MediatR;
using MongoDB.Driver;

namespace BRP.Mailer.API.Application.Mails.Queries.ListEmails;

internal sealed class ListEmailsQueryHandler : IRequestHandler<ListEmailsQuery, List<OutboxEmail>>
{
    private readonly IMongoCollection<OutboxEmail> _mongoCollection;

    public ListEmailsQueryHandler(IMongoCollection<OutboxEmail> mongoCollection)
    {
        _mongoCollection = mongoCollection;
    }
    public async Task<List<OutboxEmail>> Handle(ListEmailsQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<OutboxEmail>.Filter.Empty;
        return await _mongoCollection
            .Find(filter)
            .ToListAsync(cancellationToken: cancellationToken);      
    }
}
