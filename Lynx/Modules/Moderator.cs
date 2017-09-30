using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Methods;
using Lynx.Services.Embed;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Lynx.Modules.Settings;

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Moderator : ModuleBase<LynxContext>
    {
        static GuildConfig GuildConfig = new GuildConfig();
        OverwritePermissions MutePermissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        [Command("asar")]
        public async Task AddSelfAssignAbleRoleAsync([Remainder] IRole Role)
        {
            var Config = Context.Config;
            if (Config.Moderation.AssignableRoles.Contains(Role.Id.ToString()) == true)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Role **{Role.Name}** is already in the list.").Build()); return;
            }
            else
            {
                Config.Moderation.AssignableRoles.Add(Role.Id.ToString());
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Role **{Role.Name}** has been added to the list.").Build());
                return;
            }
        }
        [Command("rsar")]
        public async Task RemoveSelfAssignAbleRoleAsync([Remainder] IRole Role)
        {
            var Config = Context.Config;
            if (Config.Moderation.AssignableRoles.Contains(Role.Id.ToString()) == false)
            {

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Role **{Role.Name}** is not in the list.").Build()); return;
            }
            else
            {
                Config.Moderation.AssignableRoles.Remove(Role.Id.ToString());
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Role **{Role.Name}** has been removed from the list.").Build());
                return;
            }
        }
        [Command("mutelist")]
        public async Task MuteListAsync()
        {
            var Dict = Context.Config.Moderation;
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
            var Config = Context.Config.Moderation;
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
        public async Task UnmuteAsync(SocketGuildUser User)
        {
            EventsHandler.Unmuted = true;
            var Config = Context.Config;
            if (Config.Moderation.MuteList.ContainsKey(User.Id.ToString()))
            {
                IRole MuteRole = Context.Guild.GetRole(Convert.ToUInt64(Config.Moderation.MuteRoleID)) as IRole;
                Config.Moderation.MuteList.Remove(User.Id.ToString());
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
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
        public async Task Mute1Async(SocketGuildUser User, int value = 1, string time = "hour", [Remainder] string reason = "No reason has been provided.")
        {
            EventsHandler.Muted = true;
            if (User == Context.User) return;
            var UnmuteTime = Services.MuteService.Extensions.GetTime(value, time);
            var Config = Context.Config;
            if (Config.Moderation.MuteRoleID == null || Config.Moderation.MuteRoleID == "0")
            {
                var Role = await Context.Guild.CreateRoleAsync("Lynx-Mute", GuildPermissions.None, Color.DarkOrange) as IRole;
        //      await Role.ModifyAsync(x => x.Permissions = (Context.Client.CurrentUser as SocketGuildUser).Roles.Max(b => b.Permissions));
                foreach (var TextChannel in (Context.Guild as SocketGuild).TextChannels)
                {
                    try
                    {
                        await TextChannel.AddPermissionOverwriteAsync(Role, MutePermissions).ConfigureAwait(false);
                    }
                    catch { }
                }
                Config.Moderation.MuteRoleID = Role.Id.ToString();
                Config.Moderation.MuteList.Add(User.Id.ToString(), new MuteWrapper { GuildId = Context.Guild.Id.ToString(), ModeratorId = Context.User.Id.ToString(), MutedAt = DateTime.Now, Reason = reason, UnmuteTime = UnmuteTime });
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            else if (Config.Moderation.MuteRoleID != null || Config.Moderation.MuteRoleID != "0")
            {

                var Role = Context.Guild.GetRole(Convert.ToUInt64(Config.Moderation.MuteRoleID)) as IRole;
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
                Config.Moderation.MuteList.Add(User.Id.ToString(), new MuteWrapper { GuildId = Context.Guild.Id.ToString(), ModeratorId = Context.User.Id.ToString(), MutedAt = DateTime.Now, Reason = reason, UnmuteTime = UnmuteTime });
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(
                $"{User} has been succesfully **muted** from text channels.").Build());
            await Context.Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle(":zipper_mouth: User has been muted.").WithDescription(
            $"{User} has been succesfully muted.\n\n**User:** {User}\n**Unmute Date:** {UnmuteTime.ToLocalTime()}\n**Reason:** {reason}").Build());
        }
    }
}
        
    

        
   



