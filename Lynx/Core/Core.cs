using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using System;
using Lynx.Handler;
using Lynx.Database;
namespace Lynx
{
    public class Core
    {
        static void Main(string[] args) => new Core().Start().GetAwaiter().GetResult();
        DiscordSocketClient Client;
        CommandHandler Handler;
        EventsHandler EHandler;

        public async Task Start()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 500000,
                LogLevel = LogSeverity.Verbose,
            });
            await Client.CheckBotConfig();
            await EventsHandler.Load();
            await Client.LoginAsync(TokenType.Bot, Client.LoadBotConfig().BotToken);
            await Client.StartAsync();
            Client.Log += (Log) => Task.Run(() =>
            Services.Log.CLog(Services.Source.Client, Log.Message));
            var provider = ConfigureServices();
            Handler = new CommandHandler(provider);
            EHandler = new EventsHandler(provider);
            await Handler.ConfigureAsync();
            await Task.Delay(-1);
        }

        IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(Client)
                 .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            return provider;
        }
    }
}