using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace UtilityBot.Source.Commands;

public class Text : ApplicationCommandModule
{
    private readonly Dictionary<char, char> _uwuDict = new Dictionary<char, char>
    {
        { 'l', 'w' },
        { 'r', 'w' }
    };
    
    [SlashCommand("uwuify", "uwuify your text!")]
     public async Task UwUIfyCommand(
        InteractionContext ctx, 
        [Option("text", "Text to uwuify")] string text
    )
    {
        string resp = _uwuDict.Aggregate(
            text, 
            (current, pair) => current.Replace(pair.Key, pair.Value)
        );
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent(resp)
        );
    }
}