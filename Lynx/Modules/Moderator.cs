using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Threading;
using Lynx.Database;
using System.Linq;
using Lynx.Methods;
using Lynx.Handler;
using Lynx.Services.Embed;

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Moderator : ModuleBase
    {
        OverwritePermissions MutePermissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        static Timer Timer;
        [Command("mute")]
        public async Task MuteAsync(IUser User, int value, string time, [Remainder] string reason = "No reason has been provided.")
        {
            var UnmuteTime = GetTime(value, time);
            var Config = Context.Guild.LoadServerConfig();
            if (Config.Moderation.MuteRoleID == null || Config.Moderation.MuteRoleID == "0")
            {
                var Role = await Context.Guild.CreateRoleAsync("Lynx-Mute", GuildPermissions.None, Color.DarkOrange) as IRole;
                foreach (var TextChannel in (Context.Guild as SocketGuild).TextChannels)
                {
                    try
                    {
                        await TextChannel.AddPermissionOverwriteAsync(Role, MutePermissions).ConfigureAwait(false);
                    } catch { }
                }
                await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.MuteRole, Role.Id);
                await Context.Client.UpdateMuteList(User,Context.User, UpdateHandler.MuteOption.Mute, UnmuteTime, reason);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            else if(Config.Moderation.MuteRoleID != null || Config.Moderation.MuteRoleID != "0")
            {
                
                var Role = Context.Guild.GetRole(Convert.ToUInt64(Context.Guild.LoadServerConfig().Moderation.MuteRoleID)) as IRole;
                foreach (var TextChannel in (Context.Guild as SocketGuild).TextChannels)
                {
                    if (TextChannel.PermissionOverwrites.Select(x => x.Permissions).Contains(MutePermissions) == false)
                    {
                        try
                        {
                            await TextChannel.AddPermissionOverwriteAsync(Role, MutePermissions).ConfigureAwait(false);
                        }
                        catch { }
                    }
                }
                
                await Context.Client.UpdateMuteList(User, Context.User,UpdateHandler.MuteOption.Mute, UnmuteTime, reason);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle(":zipper_mouth: User has been muted.").WithDescription(
                $"{User} has been succesfully muted.\n\n**User:** {User}\n**Unmute Date:** {UnmuteTime.ToLocalTime()}\n**Reason:** {reason}").Build());
            await Context.Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle(":zipper_mouth: User has been muted.").WithDescription(
            $"{User} has been succesfully muted.\n\n**User:** {User}\n**Unmute Date:** {UnmuteTime.ToLocalTime()}\n**Reason:** {reason}").Build());
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
                       await Client.UpdateMuteList(User,null, UpdateHandler.MuteOption.Unmute);
                   }
               }));
            },null,TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(1));
        }
    }
}
        
    

        
   



