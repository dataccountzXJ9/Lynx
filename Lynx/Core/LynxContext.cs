using System;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Models.Database;

namespace Lynx.Handler

{
    public class LynxContext : ICommandContext
    {
        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketUser User { get; }
        public SocketUserMessage Message { get; }

        public ServerModel Config { get; set; }
        public LynxModel LynxConfig { get; set; }
        static LynxConfig LynxConfig_ = new LynxConfig();
        private IServiceProvider Provider;

        public LynxContext(DiscordSocketClient DiscordClient, SocketUserMessage UserMessage, IServiceProvider ServiceProvider)
        {
            Provider = ServiceProvider;
            User = UserMessage.Author;
            Guild = (UserMessage.Channel as SocketTextChannel).Guild;
            Message = UserMessage;
            Client = DiscordClient;
            Channel = UserMessage.Channel;
            Config = Provider.GetRequiredService<GuildConfig>().LoadAsync(Guild.Id);
            LynxConfig = LynxConfig_.LoadConfig;
        }
        IDiscordClient ICommandContext.Client => Client;
        IGuild ICommandContext.Guild => Guild;
        IMessageChannel ICommandContext.Channel => Channel;
        IUser ICommandContext.User => User;
        IUserMessage ICommandContext.Message => Message;
    }
}