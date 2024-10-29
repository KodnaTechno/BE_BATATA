using MongoDB.Bson;

namespace FileStorge
{
    public static class Extensions
    {
        public static FileModel Meta(this Providers.Database.File file)
        {
            return new FileModel
            {
                Name = file.Name,
                ContentType = file.ContentType,
                Id = file.FileId.ToString(),
                Size = file.Size,
                Owner = file.Owner,
                CreatedAt = file.CreatedAt,
            };
        }

        public static FileModel Meta(this BsonDocument bsonDocument, ObjectId objectId)
        {
            return new FileModel
            {
                Name = bsonDocument["Name"].AsString,
                ContentType = bsonDocument["ContentType"].AsString,
                Id = objectId.ToString(),
                Size = (int)BsonTypeMapper.MapToDotNetValue(bsonDocument["Size"]),
                Owner = bsonDocument["Owner"].AsString,
                CreatedAt = bsonDocument["CreatedAt"].ToUniversalTime(),
            };
        }
    }
}
