using System.Text.Json.Serialization;

namespace UtilityBot.Source.NekoAPI;

public struct NekoAction
{
    [JsonPropertyName("anime_name")]
    public required string AnimeName { get; set; }
    
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}

public struct NekoImage
{
    [JsonPropertyName("artist_href")]
    public required string ArtistHref { get; set; }
    
    [JsonPropertyName("artist_name")]
    public required string ArtistName { get; set; }
    
    [JsonPropertyName("source_url")]
    public required string SourceUrl { get; set; }
    
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}

public struct NekoResult<T>
{
    [JsonPropertyName("results")]
    public List<T> Results { get; set; }
}