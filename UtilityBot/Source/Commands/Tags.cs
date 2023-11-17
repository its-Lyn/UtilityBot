using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using UtilityBot.Source.Models;
using UtilityBot.Source.Services;

namespace UtilityBot.Source.Commands;

[SlashCommandGroup("tag", "manage your server's tags!")]
public class Tags : ApplicationCommandModule
{
    private readonly Mongo _mongo;

    public Tags(Mongo mongo)
    {
        _mongo = mongo;
    }

    [SlashCommand("create", "Create a tag")]
    public async Task Create(
        InteractionContext ctx, 
        [Option("Name", "The tag's name")] string name,
        [Option("Content", "The tag's content")] string content
    )
    {
        ServerTags server = await _mongo.GetServerEntry(ctx.Guild.Id.ToString());
        Tag? checkTag = server.Tags.Find(tag => tag.Name == name);
        if (checkTag is not null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                  .WithContent("A tag with that name already seems exist!")
            );
            
            return;
        }

        Tag tag = new Tag
        {
            Name = name,
            Content = content,
            
            AddedBy = ctx.Member.Id.ToString()
        };
        
        await _mongo.AddTag(ctx.Guild.Id.ToString(), tag);
        
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent($"Done! I've created the brand new tag {name}")
        );
    }
    
    [SlashCommand("erase", "Erase a tag")]
    public async Task Erase(
        InteractionContext ctx,
        [Option("Name", "The tag's name")] string name
    )
    {
        ServerTags server = await _mongo.GetServerEntry(ctx.Guild.Id.ToString());
        Tag? tag = server.Tags.Find(tag => tag.Name == name);
        if (tag is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                 .WithContent("That tag doesn't seem to exist!")
            );
            
            return;
        }

        if (tag.AddedBy != ctx.Member.Id.ToString() && (ctx.Member.Permissions & Permissions.ManageMessages) == 0)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("You don't have permission to delete that tag!")
            );
            
            return;
        }

        await _mongo.RemoveTag(ctx.Guild.Id.ToString(), name);
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent($"Removed tag {name} from this server.")
        );
    }

    [SlashCommand("use", "Use a Tag")]
    public async Task Use(
        InteractionContext ctx,
        [Option("Name", "The tag's name")] string name
    )
    {
        ServerTags server = await _mongo.GetServerEntry(ctx.Guild.Id.ToString());
        Tag? tag = server.Tags.Find(tag => tag.Name == name);
        if (tag == null)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                   .WithContent($"No tag by the name {name} exists in this server.. Maybe try to create it?")
            );

            return;
        }

        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent($"{tag.Content}")
        );
    }

    [SlashCommand("show", "Show all the tags in the server")]
    public async Task Show(InteractionContext ctx)
    {
        ServerTags server = await _mongo.GetServerEntry(ctx.Guild.Id.ToString());
        if (server.Tags.Count == 0)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                  .WithContent("There are no tags in this server!")
            );

            return;
        }
        
        string tagNames = string.Empty;
        DiscordEmbedBuilder tagEmbed = new DiscordEmbedBuilder
        {
            Title = "Server Tags",
            Color = DiscordColor.Blurple
        };

        int idx = 1;
        foreach (Tag tag in server.Tags)
        {
            tagNames += $"{idx}. {tag.Name}\n";
            idx++;
        }

        InteractivityExtension? inter = ctx.Client.GetInteractivity();
        IEnumerable<Page>? pages = inter.GeneratePagesInEmbed(tagNames, SplitType.Line, tagEmbed);
        
        await inter.SendPaginatedResponseAsync(ctx.Interaction, false, ctx.User, pages);
    }
    
    [SlashCommand("edit", "Edit a tag's contents!")]
    public async Task Edit(
        InteractionContext ctx,
        [Option("Name", "The tag's name")] string name,
        [Option("newcontent", "The tag's new content")] string content
    )
    {
        ServerTags server = await _mongo.GetServerEntry(ctx.Guild.Id.ToString());
        Tag? tag = server.Tags.Find(tag => tag.Name == name);
        if (tag is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("That tag doesn't seem to exist!")
            );
            
            return;
        }

        if (tag.AddedBy != ctx.Member.Id.ToString() && (ctx.Member.Permissions & Permissions.ManageMessages) == 0)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("You don't have permission to edit that tag!")
            );
            
            return;
        }
        
        await _mongo.EditTagContent(ctx.Guild.Id.ToString(), name, content);
        
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent($"Edited tag {name} to have the content {content}.")
        );
    }
}