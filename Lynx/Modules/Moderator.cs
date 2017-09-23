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
using System.Globalization;

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Moderator : ModuleBase
    {
        OverwritePermissions MutePermissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        [Command("mutelist")]
        public async Task MuteListAsync()
        {
            var Dict = Context.Guild.LoadServerConfig().Moderation;
            if (Dict.MuteList.Keys.Count == 0)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Mute list on {Context.Guild.Name} [{Context.Guild.Id}] is empty.").Build());
                return;
            }
            else
            {
                string UserList = null;
                var ListKey = Dict.MuteList.Keys;
                foreach (var X in ListKey)
                {
                    
                    var MuteInfo = Dict.MuteList.TryGetValue(X, out MuteWrapper Info);
                    var Diff = Info.UnmuteTime - DateTime.Now;
                    var Days = Diff.Days == 0 ? null : $" {Diff.Days.ToString()} days,";
                    var Hours = Diff.Hours == 0 ? null : $" {Diff.Hours.ToString()} hours,";
                    var Minutes = Diff.Minutes == 0 ? null : $" {Diff.Minutes.ToString()} minutes,";
                    var Seconds = Diff.Seconds == 0 ? null : $" {Diff.Seconds.ToString()} seconds";
                    var User = await Context.Guild.GetUserAsync(Convert.ToUInt64(X)) as IGuildUser;
                    UserList = UserList + User + $" [Unmuted in{Days}{Hours}{Minutes}{Seconds}]\n ";
                }
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(UserList).Build());
            }
        }
        
        [Command("muteinfo")]
        public async Task MuteInfoAsync(IUser user)
        {
            if (user == Context.User) return;
            var Config = Context.Guild.LoadServerConfig().Moderation;
            if (Config.MuteList.ContainsKey(user.Id.ToString()) == false)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{user}** is not muted.").Build()); return;
            }
            else
            {
                var MuteInfo = Config.MuteList.TryGetValue(user.Id.ToString(), out MuteWrapper Info);
                var Diff = Info.UnmuteTime - DateTime.Now;
                var Days = Diff.Days == 0 ? null : $" {Diff.Days.ToString()} days";
                var Hours = Diff.Hours == 0 ? null : $" {Diff.Hours.ToString()} hours";
                var Minutes = Diff.Minutes == 0 ? null : $" {Diff.Minutes.ToString()} minutes";
                var Seconds = Diff.Seconds == 0 ? null : $" {Diff.Seconds.ToString()} seconds";

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**User:** {user}\n**Muted by:** {(await Context.Guild.GetUserAsync(Convert.ToUInt64(Info.ModeratorId)) as IUser)}" +
                    $"\n**Muted at:** {Info.Reason}\n**Unmute at:** {Info.UnmuteTime.ToString("MM-dd HH:mm:ss", CultureInfo.InvariantCulture)} PM (in{Days}{Hours}{Minutes}{Seconds}).").Build());
            }

        }
        [Command("unmute")]
        public async Task UnmuteAsync(IUser User)
        {
            EventsHandler.Unmuted = true;
            var Config = Context.Guild.LoadServerConfig();
            if (Config.Moderation.MuteList.ContainsKey(User.Id.ToString()))
            {
                IRole MuteRole = Context.Guild.GetRole(Convert.ToUInt64(Config.Moderation.MuteRoleID)) as IRole;
                await Context.Guild.UpdateMuteList(User, null, UpdateHandler.MuteOption.Unmute);
                await (User as SocketGuildUser).RemoveRoleAsync(MuteRole);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from and text chat.").Build());
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** is not muted.").Build());
            }
        }
        [Command("mute")]
        public async Task Mute1Async(IUser User, int value = 1, string time = "hour", [Remainder] string reason = "No reason has been provided.")
        {
            EventsHandler.Muted = true;
            if (User == Context.User) return;
            var UnmuteTime = Services.Mute.Extensions.GetTime(value, time);
            var Config = Context.Guild.LoadServerConfig();
            if (Config.Moderation.MuteRoleID == null || Config.Moderation.MuteRoleID == "0")
            {
                var Role = await Context.Guild.CreateRoleAsync("Lynx-Mute", GuildPermissions.None, Color.DarkOrange) as IRole;
                foreach (var TextChannel in (Context.Guild as SocketGuild).TextChannels)
                {
                    try
                    {
                        await TextChannel.AddPermissionOverwriteAsync(Role, MutePermissions).ConfigureAwait(false);
                    }
                    catch { }
                }
                await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.MuteRole, Role.Id);
                await Context.Guild.UpdateMuteList(User, Context.User, UpdateHandler.MuteOption.Mute, UnmuteTime, reason);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            else if (Config.Moderation.MuteRoleID != null || Config.Moderation.MuteRoleID != "0")
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
                await Context.Guild.UpdateMuteList(User, Context.User, UpdateHandler.MuteOption.Mute, UnmuteTime, reason);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(
                $"{User} has been succesfully **muted** from text channels.").Build());
            await Context.Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle(":zipper_mouth: User has been muted.").WithDescription(
            $"{User} has been succesfully muted.\n\n**User:** {User}\n**Unmute Date:** {UnmuteTime.ToLocalTime()}\n**Reason:** {reason}").Build());
        }
    }
}
        
    

        
   



