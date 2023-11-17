using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using UtilityBot.Source.NekoAPI;
using UtilityBot.Source.Utilities;

namespace UtilityBot.Source.Commands;

public class Roleplay : ApplicationCommandModule
{
    private readonly NekosApi _api;
    private readonly EmbedUtils _utils;
    
    public Roleplay(NekosApi api, EmbedUtils utils)
    {
        _api = api;
        _utils = utils;
    }
    
    [SlashCommand("Hug", "Hug an user!")]
    public async Task HugCommand(
        InteractionContext ctx,
        [Option("user", "The user you want to hug.")] DiscordUser user
    )
    {
        if (await _utils.IsSelfUsed(ctx, user)) return;

        NekoResult<NekoAction> hugEndpoint = await _api.GetHug();
        DiscordEmbedBuilder embed = _utils.CreateRoleplayEmbed("hugs", hugEndpoint.Results[0].Url, ctx.Member, user);
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
        );
    }
    
    [SlashCommand("cuddle", "Cuddle an user!")]
    public async Task CuddleCommand(
        InteractionContext ctx,
        [Option("user", "The user you want to cuddle.")] DiscordUser user
    )
    {
        if (await _utils.IsSelfUsed(ctx, user)) return;

        NekoResult<NekoAction> cuddleEndpoint = await _api.GetCuddle();
        DiscordEmbedBuilder embed = _utils.CreateRoleplayEmbed("cuddles", cuddleEndpoint.Results[0].Url, ctx.Member, user);
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
        );
    }
    
    [SlashCommand("pat", "Pat an user!")]
    public async Task PatCommand(
        InteractionContext ctx,
        [Option("user", "The user you want to pat.")] DiscordUser user
    )
    {
        if (await _utils.IsSelfUsed(ctx, user)) return;

        NekoResult<NekoAction> patEndpoint = await _api.GetPat();
        DiscordEmbedBuilder embed = _utils.CreateRoleplayEmbed("pats", patEndpoint.Results[0].Url, ctx.Member, user);
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
        );
    }
}