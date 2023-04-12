using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Services.Catalog.Models;

public class Category
{
    [BsonId]    // Mongo Db Identity Id olarak görmesi için
    [BsonRepresentation(BsonType.ObjectId)] // Mongo Db tarafında tutulan ObjectID'yi string olarak algılar.
    public string Id { get; set; }
    public string Name { get; set; }
}
