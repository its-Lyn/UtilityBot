using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace UtilityBot.Source.Commands;

public class MiscCommands : ApplicationCommandModule
{
    [SlashCommand("help", "Get information about commands")]
    public async Task HelpAsync(InteractionContext ctx)
    {
        DiscordEmbedBuilder helpEmbed = new DiscordEmbedBuilder
        {
            Color = DiscordColor.Blue,
            Title = "UtilityBot Help",
            Description = "UtilityBot is a general purpose bot that provides fun for your Discord server!"
        };

        helpEmbed.AddField(
            "Image Commands",
            "**/neko** *<count> (defaults to 1)* - Sends a cute image of a neko >~<"
        );
        
        helpEmbed.AddField(
            "Roleplay Commands",
            "**/cuddle** *<user>* - Cuddle an user! Adorable~\n" + 
            "**/hug** *<user>* - Hug an user! Cute!\n" +
            "**/pat** *<user>* - Pat an user! Awwh~"
        );
        
        helpEmbed.AddField(
            "Tag Commands",
            "**/tag create** *<name>* *<content>* - Create a tag you can use at any time!\n" +
            "**/tag erase** *<name>* - Delete one of your tags; or another user's given you have permissions\n" +
            "**/tag edit** *<name>* - Delete one of your tags; or another user's given you have permission\n" +
            "**/tag show** - Shows all the tag names created in the current server\n" +
            "**/tag use** *<name>* - Use one of the server's tags!\nYou can also type **.tag** *<name>* inside a channel to access tags easier!"
        );

        helpEmbed.AddField(
            "Fun Commands",
            "**/uwuify** *<text>* - UwUify a message~ :3\n" +
            "**/invite** - Invite me to your server!"
        );

        helpEmbed.WithThumbnail(ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png));
        
        helpEmbed.WithTimestamp(DateTime.Now);
        helpEmbed.WithAuthor(ctx.User.Username, iconUrl: ctx.User.GetAvatarUrl(ImageFormat.Png));
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder()
                .AddEmbed(helpEmbed)
                .AsEphemeral()
        );
    }

    [SlashCommand("invite", "invite me to your server!")]
    public async Task Invite(InteractionContext ctx)
    {
        const string invite = "https://www.google.com/search?q=troll+face";
        DiscordEmbedBuilder inviteEmbed = new DiscordEmbedBuilder
        {
            Color = DiscordColor.Purple,
            Title = "Thank you!",
            Description = "Thank you for deciding on inviting me!\nClick \"Thank you!\" to invite me to your server.",
            
            Url = invite
        };
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder()
                .AddEmbed(inviteEmbed)
        );
    }
}