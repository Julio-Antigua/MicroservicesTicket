using Common.Core.Events;
using MongoDB.Bson.Serialization.Attributes;
using Ticketing.Command.Domain.Common;

namespace Ticketing.Command.Domain.EventModels;

[BsonCollection("eventStore")]
public class EventModel : Document
{
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("aggregateIdentifier")]
    public required string AggregateIdentifier { get; set; }

    [BsonElement("aggregateType")]
    public string AggregateType { get; set; } = string.Empty;

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("eventType")]
    public string EventType { get; set; } = string.Empty;
    [BsonElement("eventData")]
    public BaseEvent? EventData { get; set; }
}