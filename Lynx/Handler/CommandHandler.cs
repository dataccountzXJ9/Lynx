using System;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Reflection;
using Lynx.Database;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Discord;
using Lynx.Methods;
using Lynx.Services.Embed;

namespace Lynx.Handler
{
    public class CommandHandler
    {
        DiscordSocketClient Client;
        IServiceProvider provider;
        CommandService commands;
        public CommandHandler(IServiceProvider Prov)
        {
            provider = Prov;
            Client = provider.GetService<DiscordSocketClient>();
            commands = provider.GetService<CommandService>();
            Client.MessageReceived += async (Message) => { await HandleCommand(Message); await NSFW.NSFWService.NSFWImplementation(Message); };
        }
        public async Task ConfigureAsync()
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        public async Task HandleCommand(SocketMessage Message)
        {
            int argPos = 0;
            var Guild = (Message.Channel as SocketTextChannel).Guild;
            var Context = new SocketCommandContext(Client, Message as SocketUserMessage);
            if (!(Message as SocketUserMessage).HasStringPrefix(Guild.GetPrefix().ToLowerInvariant(), ref argPos)) return;
            var Result = await commands.ExecuteAsync(Context, argPos, provider);
            if (Result.IsSuccess)
            {
                await Context.Message.DeleteAsync();
                EventsHandler.Deleted = true;
            }
            if (!Result.IsSuccess)
            {
                if (Context.Client.LoadBotConfig().Debug == true)
                {
                    await Context.Channel.SendMessageAsync(Result.ErrorReason);
                }
                else
                {
                    switch (Result.Error)
                    {
                        case CommandError.UnmetPrecondition:
                            await Message.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("Unknown precondition failed.").Build()); return;
                        case CommandError.Exception:
                            await Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithTitle("Exception occured in #" + (Message.Channel as SocketTextChannel).Name)
                                .WithDescription("**In Channel:** " + (Message.Channel as SocketTextChannel).Mention + $" [{Message.Channel.Id}]\n"
                                + "**Parameters: **" + Message.Content + "\n"
                                + "**Fired by:** " + Message.Author + $"\n**Exception**```{Result.ErrorReason}```").Build());
                            return;
                        case CommandError.BadArgCount:
                            await Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithTitle("Command failed in #" + (Message.Channel as SocketTextChannel).Name)
                            .WithDescription("**In Channel:** " + (Message.Channel as SocketTextChannel).Mention + $" [{Message.Channel.Id}]\n"
                            + "**Parameters: **" + Message.Content + "\n"
                            + "**Fired by:** " + Message.Author).Build());
                            return;
                        case CommandError.MultipleMatches:
                        case CommandError.ObjectNotFound:
                        case CommandError.ParseFailed:
                        case CommandError.UnknownCommand:
                        case CommandError.Unsuccessful:
                            return;
                    }
                }
            }
        }
    }
}
