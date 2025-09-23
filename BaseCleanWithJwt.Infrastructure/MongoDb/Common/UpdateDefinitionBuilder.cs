using MongoDB.Driver;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Common;

public static class UpdateDefinitionBuilder
{
    public static UpdateDefinition<T> Create<T>(T updatedEntity, params string[] excludeFields)
    {
        var updateBuilder = Builders<T>.Update;
        var updates = new List<UpdateDefinition<T>>();
        var excludeSet = new HashSet<string>(excludeFields);

        foreach (var property in typeof(T).GetProperties())
        {
            if (excludeSet.Contains(property.Name) || !property.CanRead)
                continue;

            var value = property.GetValue(updatedEntity);

            if (IsNullOrDefault(value!))
                continue;

            updates.Add(updateBuilder.Set(property.Name, value));
        }

        if (updates.Count == 0)
            throw new InvalidOperationException("No properties to update after applying exclusions.");

        return updateBuilder.Combine(updates);
    }

    private static bool IsNullOrDefault(object value)
    {
        if (value == null) return true;

        var type = value.GetType();
        if (type.IsValueType)
            return value.Equals(Activator.CreateInstance(type));

        return type == typeof(string) && string.IsNullOrEmpty((string)value);
    }
}