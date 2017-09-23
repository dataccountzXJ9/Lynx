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
using Lynx.Services.Help;

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Moderator : ModuleBase
    {
        OverwritePermissions MutePermissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        [Command("iam")]
        public async Task IAmAsync(IRole Role)
        {
            EventsHandler.Muted = true;
            var Config = Context.Guild.LoadServerConfig().Moderation;
            if (Config.AssignableRoles.Contains(Role.Id.ToString()))
            {
                await (Context.User as SocketGuildUser).AddRoleAsync(Role);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You now have **{Role.Name}**.").Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Role.Name}** is not self-assignable.").Build());
                return;
            }
        }
        [Command("iamn")]
        public async Task IAmnAsync(IRole Role)
        {
            EventsHandler.Unmuted = true;
            var Config = Context.Guild.LoadServerConfig().Moderation;
            if (Config.AssignableRoles.Contains(Role.Id.ToString()))
            {
                await (Context.User as SocketGuildUser).RemoveRoleAsync(Role);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You no longer have **{Role.Name}**.").Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Role.Name}** is not self-assignable.").Build());
                return;
            }
        }
        [Command("lsar")]
        public async Task ListSelfAssignAbleRolesAsync()
        {
            int i = 0;
            int j = 0;
            var Config = Context.Guild.LoadServerConfig().Moderation.AssignableRoles;
            if(Config.Count == 0)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Context.Guild.Name}'s** selfassignablelist is **empty**.").Build());
                return;
            }
            IList<IRole> RoleList = new List<IRole>();
            foreach(var Role in Config)
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if(Role_ == null)
                {
                    await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.RemoveSelfassignableRole, Convert.ToUInt64(Role));
                    return;
                }
                else
                {
                    RoleList.Add(Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole);
                }
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Assignable roles for **{Context.Guild.Name}**\n" + $@"```css
{string.Join("\n",RoleList.GroupBy(x=> (i++)/ 3).Select(L9=>string.Concat(L9.Select(F=>$"{F.Name, -15}"))))}
```").Build());
        }
        [Command("asar")]
        public async Task AddSelfAssignAbleRoleAsync([Remainder] IRole Role)
        {
            var Config = Context.Guild.LoadServerConfig();
            if (Config.Moderation.AssignableRoles.Contains(Role.Id.ToString()) == true)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Role **{Role.Name}** is already in the list.").Build()); return;
            }
            else
            {
                await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.AddSelfassignableRole, Role.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Role **{Role.Name}** has been added to the list.").Build());
                return;
            }
        }
        [Command("rsar")]
        public async Task RemoveSelfAssignAbleRoleAsync([Remainder] IRole Role)
        {
            var Config = Context.Guild.LoadServerConfig();
            if (Config.Moderation.AssignableRoles.Contains(Role.Id.ToString()) == false)
            {

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Role **{Role.Name}** is not in the list.").Build()); return;
            }
            else
            {
                await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.RemoveSelfassignableRole, Role.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Role **{Role.Name}** has been removed from the list.").Build());
                return;
            }
        }
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
        
    

        
   



