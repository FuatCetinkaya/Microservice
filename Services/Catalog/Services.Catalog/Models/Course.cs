using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Services.Catalog.Models;

public class Course
{
    [BsonId]    // Mongo Db Identity Id olarak görmesi için
    [BsonRepresentation(BsonType.ObjectId)] // Mongo Db tarafında tutulan ObjectID'yi string olarak algılar.
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
    public string Picture { get; set; }
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedTime { get; set; }

    public Feature Feature { get; set; }    // Feature ile One to One ilişki 

    [BsonRepresentation(BsonType.ObjectId)]
    public string CategoryId { get; set; }  // Category ile One To Many ilişki
    [BsonIgnore] // Mongo db'de collection'lara koyarken gözardı et. Ben bunu kod kısmında kullanacağım.
    public Category Category { get; set; }

}
