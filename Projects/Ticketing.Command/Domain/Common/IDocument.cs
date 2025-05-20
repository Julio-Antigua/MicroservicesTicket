using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ticketing.Command.Domain.Common
{
    public interface IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        ObjectId Id { get; set; }
    }
}
