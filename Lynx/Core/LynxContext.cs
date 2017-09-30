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
        public IUser User { get; }
        public IGuild Guild { get; }
        public IUserMessage Message { get; }
        public IDiscordClient Client { get; }
        public IMessageChannel Channel { get; }
        public ServerModel Config { get; set; }
        public LynxModel LynxConfig { get; set; }
        static LynxConfig LynxConfig_ = new LynxConfig();
        private IServiceProvider Provider;

        public LynxContext(IDiscordClient DiscordClient, IUserMessage UserMessage, IServiceProvider ServiceProvider)
        {
            Provider = ServiceProvider;
            User = UserMessage.Author;
            Guild = (UserMessage.Channel as IGuildChannel).Guild;
            Message = UserMessage;
            Client = DiscordClient;
            Channel = UserMessage.Channel;
            Config = Provider.GetRequiredService<GuildConfig>().LoadAsync(Guild.Id);
            LynxConfig = LynxConfig_.LoadConfig;
        }
    }
}