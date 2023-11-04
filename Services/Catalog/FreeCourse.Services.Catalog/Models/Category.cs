using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    public class Category
    {

        [BsonId]//mongodb haber ettik id oldugunu
        [BsonRepresentation(BsonType.ObjectId)]//tipini söylüyoru<
        public string Id { get; set; } //id string olmalı çünkü mongodb otomatik üretecek
        public string Name { get; set; }
    }
}
