using BaseCleanWithJwt.Domain.Common.Generic;
using MongoDB.Driver;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Common;

public static class FilterDefinitionBuilder
{
    public static FilterDefinition<T> Create<T>(FilterBase filter)
    {
        var builder = Builders<T>.Filter;
        var filterDefinition = builder.Empty;

        if (filter.From.HasValue)
        {
            var fromDate = DateTimeOffset.FromUnixTimeMilliseconds(filter.From.Value).UtcDateTime;
            filterDefinition &= builder.Gte("CreatedAt", fromDate);
        }

        if (filter.To.HasValue)
        {
            var toDate = DateTimeOffset.FromUnixTimeMilliseconds(filter.To.Value).UtcDateTime;
            filterDefinition &= builder.Lte("CreatedAt", toDate);
        }

        return filterDefinition;
    }
}
