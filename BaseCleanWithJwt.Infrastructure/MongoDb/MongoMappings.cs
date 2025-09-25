using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using BaseCleanWithJwt.Domain.Entities;
using BaseCleanWithJwt.Domain.Common;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Reflection;

namespace BaseCleanWithJwt.Infrastructure.MongoDb;

public static class MongoMappings
{
    private static bool _registered = false;

    public static void Register()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); // set global Guid representation to Standard
        if (_registered) return;
        _registered = true;

        var domainAssembly = typeof(BaseModel).Assembly;

        var entityTypes = domainAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseModel).IsAssignableFrom(t));

        foreach (var type in entityTypes)
        {
            if (BsonClassMap.IsClassMapRegistered(type)) continue;

            // buat instance BsonClassMap<T> secara dinamis
            var classMapType = typeof(BsonClassMap<>).MakeGenericType(type);
            var classMap = (BsonClassMap)Activator.CreateInstance(classMapType)!;

            classMap.AutoMap();

            var idProp = type.GetProperty(nameof(BaseModel.Id),
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (idProp != null)
            {
                classMap.MapMember(idProp)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard))
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetIsRequired(true);
            }

            BsonClassMap.RegisterClassMap(classMap);
        }

    }
}

