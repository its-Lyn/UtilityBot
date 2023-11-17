using System.Data;
using MongoDB.Driver;
using UtilityBot.Source.Models;
using Tag = UtilityBot.Source.Models.Tag;

namespace UtilityBot.Source.Services;

public class Mongo
{
    private MongoClientSettings Settings { get; }
    private MongoClient MongoClient { get; }
    private IMongoDatabase BotDatabase { get; }

    public Mongo()
    {
        string? connection = Environment.GetEnvironmentVariable("UBCon");
        if (string.IsNullOrEmpty(connection))
        {
            throw new NoNullAllowedException("Token cannot be found or is empty");
        }
        
        Settings = MongoClientSettings.FromConnectionString(connection);
        Settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        
        MongoClient = new MongoClient(Settings);
        BotDatabase = MongoClient.GetDatabase("UtilityBot");
    }

    public async Task<ServerTags> GetServerEntry(string serverId)
    {
        IMongoCollection<ServerTags> collection = BotDatabase.GetCollection<ServerTags>("Tags");
        
        ServerTags? server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        if (server is not null) 
            return server;
        
        await CreateServerEntry(serverId);
        server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();

        return server;
    }

    private async Task CreateServerEntry(string serverId)
    {
        IMongoCollection<ServerTags> collection = BotDatabase.GetCollection<ServerTags>("Tags");
        await collection.InsertOneAsync(
            new ServerTags
            {
                ServerId = serverId,
                Tags = new List<Tag>()
            }
        );
    }

    public async Task AddTag(string serverId, Tag tag)
    {
        IMongoCollection<ServerTags> collection = BotDatabase.GetCollection<ServerTags>("Tags");
        
        // Find current entry
        ServerTags? server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        if (server is null)
        {
            await CreateServerEntry(serverId);
            server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        }
        
        // Manipulate the current server tag entry...
        List<Tag> tags = server.Tags;
        tags.Add(tag);
        
        // And finally, "update" it.
        await collection.FindOneAndReplaceAsync(
            s => s.ServerId == serverId,
            new ServerTags
            {
                ServerId = serverId,
                Tags = tags
            }
        ); 
    }

    public async Task RemoveTag(string serverId, string tagName)
    {
        IMongoCollection<ServerTags> collection = BotDatabase.GetCollection<ServerTags>("Tags");
        
        ServerTags? server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        if (server is null)
        {
            await CreateServerEntry(serverId);
            server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        }
        
        List<Tag> tags = server.Tags;
        Tag? tag = tags.Find(tag => tag.Name == tagName);
        if (tag is not null) tags.Remove(tag);        
        
        await collection.FindOneAndReplaceAsync(
            s => s.ServerId == serverId,
            new ServerTags
            {
                ServerId = serverId,
                Tags = tags
            }
        );
    }

    public async Task EditTagContent(string serverId, string name, string content)
    {
        IMongoCollection<ServerTags> collection = BotDatabase.GetCollection<ServerTags>("Tags");
        
        ServerTags? server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        if (server is null)
        {
            await CreateServerEntry(serverId);
            server = await collection.Find(s => s.ServerId == serverId).FirstOrDefaultAsync();
        }
        
        List<Tag> tags = server.Tags;
        Tag? tag = tags.Find(tag => tag.Name == name);
        if (tag is null) return;

        tag.Content = content;
        
        await collection.FindOneAndReplaceAsync(
            s => s.ServerId == serverId,
            new ServerTags
            {
                ServerId = serverId,
                Tags = tags
            }
        );
    }
}