using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using UtilityBot.Source.NekoAPI;

namespace UtilityBot.Source.Commands;

public class Images : ApplicationCommandModule
{
    private readonly NekosApi _api;
    public Images(NekosApi api)
    {
        _api = api;
    }
    
    [SlashCommand("neko", "Get images of nekos!")]
    public async Task Neko(
        InteractionContext ctx, 
        [Option("count", "The amount of images to fetch, up to 5")] long count = 1
    )
    {
        if (count > 5)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                  .WithContent("You can only fetch up to 5 images at a time!")
            );
            
            return;
        }

        string urls = string.Empty;
        NekoResult<NekoImage> catGirls = await _api.GetNeko(count);
        for (int i = 0; i < count; i++)
            urls += $"{i+1}. {catGirls.Results[i].Url}\n";
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
              .WithContent(urls)
        );
    }
}