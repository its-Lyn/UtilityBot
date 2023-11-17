using MongoDB.Bson.Serialization.Attributes;

namespace UtilityBot.Source.Models;

public class Tag
{
    public required string Name { get; set; }
    public required string Content { get; set; }
    
    public required string AddedBy { get; set; }
}

[BsonIgnoreExtraElements]
public class ServerTags
{
    [BsonElement("ServerID")]
    public required string ServerId { get; set; }
    
    public required List<Tag> Tags { get; set; }
}