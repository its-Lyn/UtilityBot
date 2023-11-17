using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using UtilityBot.Source.Models;
using UtilityBot.Source.Services;

namespace UtilityBot.Source.Commands.LegacyCommands;

public class TagsLegacy : BaseCommandModule
{
    private readonly Mongo _mongo;

    public TagsLegacy(Mongo mongo)
    {
        _mongo = mongo;
    }
    
    [Command("tag")]
    public async Task Tag(
        CommandContext ctx,
        [RemainingText] string name
    )
    {
        ServerTags server = await _mongo.GetServerEntry(ctx.Guild.Id.ToString());
        Tag? tag = server.Tags.Find(tag => tag.Name == name);
        if (tag == null)
        {
            await ctx.RespondAsync($"No tag by the name {name} exists in this server.. Maybe try to create it?");
            return;
        }
        
        await ctx.RespondAsync(tag.Content);
    }
}