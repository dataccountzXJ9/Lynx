using Discord;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Methods;
using Lynx.Services.Embed;
using NLog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lynx.Handler
{
    public class MuteHandler
    {
        static Events Events = new Events();
        static GuildConfig GuildConfig = new GuildConfig();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public MuteHandler()
        {
            Events.OnUserUnmuted += OnUserUnmuted;
        }
        public static Task InvokeUserUnmuted(SocketGuild Guild, SocketGuildUser User)
        {
            Events.InvokeOnUserUnmuted(Guild, User);
            return Task.CompletedTask;
        }
        private void OnUserUnmuted(SocketGuild Guild, SocketGuildUser User)
        {
            Task.Run(async () =>
             {
                 var Config = GuildConfig.LoadAsync(Guild.Id);
                 await (User as SocketGuildUser).RemoveRoleAsync((Guild.GetRole(Convert.ToUInt64(Config.Moderation.MuteRoleID))));
                 await Guild.GetJoinLogChannel().SendMessageAsync($"", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from text and voice chat.").WithFooter(x =>
                 {
                     x.Text = $"{User} | [Automatic Message]";
                     x.IconUrl = User.GetAvatarUrl();
                 }).Build());

                 await Guild.GetLogChannel().SendMessageAsync($"", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from text and voice chat.").WithFooter(x =>
                 {
                     x.Text = $"{User} | [Automatic Message]";
                     x.IconUrl = User.GetAvatarUrl();
                 }).Build());
             });
        }
        static Timer Timer;
        internal static Task MuteService(DiscordSocketClient Client)
        {
            logger.Info("Mute service has been created.");
            Timer = new Timer(_ =>
            {
                foreach (var Guild in Client.Guilds)
                {
                    var Config = GuildConfig.LoadAsync(Guild.Id);
                    var MuteList = Config.Moderation.MuteList;
                    Task.WhenAll(MuteList.Select(async snc =>
                    {
                        if (DateTime.Now > snc.Value.UnmuteTime)
                        {
                            var Config_ = GuildConfig.LoadAsync(Convert.ToUInt64(snc.Value.GuildId));
                            EventsHandler.Unmuted = true;
                            var User = Guild.GetUser(Convert.ToUInt64(snc.Key)) as SocketGuildUser;
                            await InvokeUserUnmuted(Guild, User);
                            Config_.Moderation.MuteList.Remove(User.Id.ToString());
                            await GuildConfig.SaveAsync(Config_, Guild.Id);
                        }
                    }));
                }
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

    }
    public class Events
    {
        public event Action<SocketGuild, SocketGuildUser> OnUserUnmuted = delegate { };
        public Task InvokeOnUserUnmuted(SocketGuild Guild, SocketGuildUser User)
        {
            OnUserUnmuted?.Invoke(Guild, User);
            return Task.CompletedTask;
        }
    }
}
