using Discord;
using Discord.WebSocket;
using Lynx.Handler;
using Lynx.Methods;
using Lynx.Services.Embed;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Lynx.Services.Mute
{
    public static class Extensions
    {
        static Timer Timer;
        public static void RemoveUser(DiscordSocketClient Client)
        {
            Timer = new Timer(_ =>
            {
                var Config = Client.LoadMuteList();
                var Mutelist = Config.MuteList;
                Task.WhenAll(Mutelist.Select(async snc =>
                {
                    if (DateTime.Now > snc.Value.UnmuteTime)
                    {
                        var Guild = Client.GetGuild(Convert.ToUInt64(snc.Value.GuildId));
                        var User = Client.GetGuild(Convert.ToUInt64(snc.Value.GuildId)).GetUser(Convert.ToUInt64(snc.Key)) as SocketGuildUser;
                        await (User as SocketGuildUser).RemoveRoleAsync((Guild.GetRole(Convert.ToUInt64(Guild.LoadServerConfig().Moderation.MuteRoleID))));
                        await User.Guild.GetLogChannel().SendMessageAsync($"", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from text and voice chat.").WithFooter(x =>
                        {
                            x.Text = $"{User} | [Automatic Message]";
                            x.IconUrl = User.GetAvatarUrl();
                        }).Build());
                        await User.GetOrCreateDMChannelAsync().Result.SendMessageAsync("");
                        await Client.UpdateMuteList(User, null, UpdateHandler.MuteOption.Unmute);
                    }
                }));
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
        public static DateTime GetTime(int Value, string Time)
        {
            var DT = DateTime.Now;
            switch (Time.ToLowerInvariant())
            {
                case "hour":
                case "hours":
                    return DT.AddHours(Value);
                case "minutes":
                case "minute":
                    return DT.AddMinutes(Value);
                case "day":
                case "days":
                    return DT.AddDays(Value);
                case "second":
                case "seconds":
                    return DT.AddSeconds(Value);
                default:
                    return DT;
            }
        }
    }
}
