using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Interactive;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Threading.Tasks;
namespace Lynx
{
    public class Core
    {
        static void Main(string[] args) => new Core().Start().GetAwaiter().GetResult();
        DiscordSocketClient Client;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static LynxConfig LynxConfig = new LynxConfig();
        public async Task Start()
        {
            await LynxConfig.LoadConfigAsync();
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                    MessageCacheSize = 500000,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Info,
            });
            ConfigureServices();
            await Client.LoginAsync(TokenType.Bot, LynxConfig.LoadConfig.BotToken);
            await Client.StartAsync();
            Client.Log += (Log) => Task.Run(() =>
            logger.Log(LogLevel.Info, Log.Message));
            await MuteHandler.MuteService(Client);
      //    await RemindMeHandler.RemindMeService(Client);
            await Task.Delay(-1);
        }
        public async void ConfigureServices()
        {
            var services = new ServiceCollection()
            .AddSingleton(Client)
            .AddSingleton(new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async }))
            .AddSingleton<CommandHandler>()
            .AddSingleton<LynxConfig>()
            .AddSingleton<GuildConfig>()
            .AddSingleton(new InteractiveService(Client));
            var Provider = services.BuildServiceProvider();
            LynxBase<LynxContext>.Provider = Provider;
            services.AddSingleton(new EventsHandler(Provider));
            await Provider.GetRequiredService<CommandHandler>().ConfigureAsync(Provider);
            logger.Log(LogLevel.Info, "Services have been loaded.");
        }
    }
}