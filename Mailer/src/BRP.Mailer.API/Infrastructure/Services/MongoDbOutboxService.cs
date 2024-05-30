using BRP.Mailer.API.Application.Integrations;
using BRP.Mailer.API.Domain;
using MongoDB.Driver;

namespace BRP.Mailer.API.Infrastructure.Services;

internal sealed class MongoDbOutboxService : IOutboxService
{
    private readonly IMongoCollection<OutboxEmail> _mongoCollection;

    public MongoDbOutboxService(IMongoCollection<OutboxEmail> mongoCollection)
    {
        _mongoCollection = mongoCollection;
    }

    public async Task<OutboxEmail?> GetUnprocessedEmailAsync()
    {
        var filter = Builders<OutboxEmail>.Filter.Eq(x => x.DateTimeUtcProcessed, null);
        return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task QueueEmailAsync(OutboxEmail emailOutboxEntity)
    {
        await _mongoCollection.InsertOneAsync(emailOutboxEntity);
    }
}
