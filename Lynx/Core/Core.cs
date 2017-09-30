using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
namespace Lynx
{
    public class Core
    {
        static void Main(string[] args) => new Core().Start().GetAwaiter().GetResult();
        DiscordSocketClient Client;
        static LynxConfig LynxConfig = new LynxConfig();
        public async Task Start()
        {
            await LynxConfig.LoadConfigAsync();
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 500000,
                LogLevel = LogSeverity.Verbose,
            });
            ConfigureServices();
            await Client.LoginAsync(TokenType.Bot, LynxConfig.LoadConfig.BotToken);
            await Client.StartAsync();
            Client.Log += (Log) => Task.Run(() =>
            Services.Log.CLog(Services.Source.Client, Log.Message));
            await MuteHandler.MuteService(Client);
            await Task.Delay(-1);
        }
        public async void ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(Client)
                 .AddSingleton(new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<LynxConfig>()
                .AddSingleton<GuildConfig>();
                var Provider = services.BuildServiceProvider();
                LynxBase<LynxContext>.Provider = Provider;
            services.AddSingleton(new EventsHandler(Provider));
            await Provider.GetRequiredService<CommandHandler>().ConfigureAsync(Provider);
        }
    }
}