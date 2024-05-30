﻿namespace BRP.Mailer.API.Infrastructure;

internal sealed class MongoDBSettings
{
    public const string SectionName = "MongoDBSettings";
    public string ConnectionString { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
}
