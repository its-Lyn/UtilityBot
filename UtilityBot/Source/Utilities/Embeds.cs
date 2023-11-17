using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using UtilityBot.Source.NekoAPI;

namespace UtilityBot.Source.Utilities;

public class EmbedUtils
{
    private readonly DiscordClient _client;
    
    public EmbedUtils(DiscordClient client)
    {
        _client = client;
    }
    
    public DiscordEmbedBuilder CreateRoleplayEmbed(
        string action,
        string imageUrl,
        DiscordUser self,
        DiscordUser member
    )
    {
        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
        {
            Title = $"{string.Concat(action[0].ToString().ToUpper(), action.AsSpan(1))}! {DiscordEmoji.FromName(_client, ":hug:")}",
            Description = $"<@{self.Id}> {action} <@{member.Id}>!",
            
            Color = DiscordColor.Orange,
            
            ImageUrl = imageUrl
        };
        
        return embed;
    }

    public async Task<bool> IsSelfUsed(InteractionContext ctx, DiscordUser user)
    {
        if (ctx.Member.Id != user.Id) return false;
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("You can't do that...")
        );
            
        return true;
    }
}