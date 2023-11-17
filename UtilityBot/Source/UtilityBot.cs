using System.Data;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using UtilityBot.Source.Commands.LegacyCommands;
using UtilityBot.Source.NekoAPI;
using UtilityBot.Source.Services;
using UtilityBot.Source.Utilities;

namespace UtilityBot.Source;

public static class Bot
{
    public static IServiceProvider Services = null!;
    
    private static string GetToken()
    {
        string? token = Environment.GetEnvironmentVariable("UBTOKEN");
        if (string.IsNullOrEmpty(token))
        {
            throw new NoNullAllowedException("Token cannot be found or is empty");
        }
        
        return token;
    }

    public static async Task Main()
    {
        string token = GetToken();

        DiscordClient client = new DiscordClient(new DiscordConfiguration
        {
            Token = token,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents | DiscordIntents.GuildPresences
        });
        
        client.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromSeconds(30)
        });
        
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(client);
        serviceCollection.AddSingleton<NekosApi>();
        serviceCollection.AddSingleton<EmbedUtils>();
        serviceCollection.AddSingleton<Mongo>();
        
        Services = serviceCollection.BuildServiceProvider();

        CommandsNextExtension legacyCommands = client.UseCommandsNext(new CommandsNextConfiguration
        {
            StringPrefixes = new[] { "." },
            EnableDms = false,
            EnableMentionPrefix = false,
            EnableDefaultHelp = false,
            
            Services = Services
        });
        
        legacyCommands.RegisterCommands<TagsLegacy>();
        
        SlashCommandsExtension slashCommands = client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = Services
        });
        
        slashCommands.RegisterCommands(Assembly.GetExecutingAssembly());
        
        slashCommands.SlashCommandErrored += async (_, e) => {
            Console.WriteLine(e.Exception.Message);

            try 
            {                
                await e.Context.CreateResponseAsync(
                    "An error occurred while executing the command!"
                );
            } 
            catch (DSharpPlus.Exceptions.BadRequestException)
            {
                await e.Context.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("An error occurred while executing the command!")
                );  
            }
        };

        DiscordActivity activity = new DiscordActivity
        {
            ActivityType = ActivityType.Watching,
            Name = "Mountain... (/help)"
        };
        
        client.Ready += async (_, _)
            => await client.UpdateStatusAsync(activity);
        
        await client.ConnectAsync();
        
        using (SemaphoreSlim sem = new SemaphoreSlim(0, 1))
        {
            Console.CancelKeyPress += (_, e) => {
                sem.Release();
                e.Cancel = true;
            };
            
            await sem.WaitAsync();
        }
        
        await client.DisconnectAsync();
    }
}