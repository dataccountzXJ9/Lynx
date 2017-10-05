using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Interactive;
using Lynx.Methods;
using Lynx.Services.Embed;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Lynx.Modules
{
    public class Administration : LynxBase<LynxContext>
    {
        OverwritePermissions MutePermissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        static GuildConfig GuildConfig = new GuildConfig();
        private static readonly PaginatedAppearanceOptions AOptions = new PaginatedAppearanceOptions()
        {
            Timeout = TimeSpan.FromMinutes(60)
        };
        [Command("customreactions")]
        public async Task ToggleCustomReactions()
        {

        }
        [Command("iam")]
        public async Task IAmAsync([Remainder] IRole Role)
        {
            EventsHandler.Muted = true;
            var Config = Context.Config.Moderation;
            if (Config.AssignableRoles.Contains(Role.Id.ToString()))
            {
                try
                {
                    await (Context.User as SocketGuildUser).AddRoleAsync(Role);
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"I can not assign **{Role.Name}** because it is higher than my role.").Build());
                    return;
                }
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You now have **{Role.Name}**.").Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Role.Name}** is not self-assignable.").Build());
                return;
            }
        }
        [Command("iamn")]
        public async Task IAmnAsync([Remainder] IRole Role)
        {
            EventsHandler.Unmuted = true;
            var Config = Context.Config.Moderation;
            if (Config.AssignableRoles.Contains(Role.Id.ToString()))
            {
                try
                {
                    await (Context.User as SocketGuildUser).RemoveRoleAsync(Role);
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"I can not assign **{Role.Name}** because it is higher than my role.").Build());
                    return;
                }
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You no longer have **{Role.Name}**.").Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Role.Name}** is not self-assignable.").Build());
                return;
            }
        }
        [Command("lsar")]
        public async Task ListSelfAssignAbleRolesAsync1()
        {
            var Config = Context.Config;
            if (Config.Moderation.AssignableRoles.Count == 0)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Context.Guild.Name}'s** self asisgnable role list is **empty**.").Build());
                return;
            }
            string Page1 = null;
            string Page2 = null;
            string Page3 = null;
            string Page4 = null;
            string Page5 = null;
            string Page6 = null;
            string Page7 = null;
            string Page8 = null;
            string Page9 = null;
            string Page10 = null;
            string Page11 = null;
            string Page12 = null;
            string Page13 = null;
            string Page14 = null;
            string Page15 = null;
            foreach (var Role in Config.Moderation.AssignableRoles.Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if(Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page1 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(10).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page2 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(20).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page3 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(30).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page4 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(40).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page5 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(50).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page6 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(60).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page7 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(70).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page8 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(80).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page9 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(90).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page10 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(100).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page11 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(110).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page12 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(120).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page13 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(130).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page14 += "• " + Role_.Name + "\n"; }
            }
            foreach (var Role in Config.Moderation.AssignableRoles.Skip(140).Take(10))
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null) { Config.Moderation.AssignableRoles.Remove(Role); }
                else { Page15 += "• " + Role_.Name + "\n"; }
            }
            List<string> Pages = new List<string>();
            if (Page1 != null)
                Pages.Add(Page1);
            if (Page2 != null)
                Pages.Add(Page2);
            if (Page3 != null)
                Pages.Add(Page3);
            if (Page4 != null)
                Pages.Add(Page4);
            if (Page5 != null)
                Pages.Add(Page5);
            if (Page6 != null)
                Pages.Add(Page6);
            if (Page7 != null)
                Pages.Add(Page7);
            if (Page8 != null)
                Pages.Add(Page8);
            if (Page9 != null)
                Pages.Add(Page9);
            if (Page10 != null)
                Pages.Add(Page10);
            if (Page11 != null)
                Pages.Add(Page11);
            if (Page12 != null)
                Pages.Add(Page12);
            if (Page13 != null)
                Pages.Add(Page13);
            if (Page14 != null)
                Pages.Add(Page14);
            if (Page15 != null)
                Pages.Add(Page15);
            var PaginatedMessage = new PaginatedMessage()
            {
                Author = new Discord.EmbedAuthorBuilder()
                {
                    IconUrl = Context.Guild.IconUrl,
                    Name = Context.Guild.Name + " Self assignable roles",
                },
                Options = AOptions,
                Pages = Pages,
                Color = Color.Green,
            };
            await PagedReplyAsync(PaginatedMessage);
        }
        [Command("asar")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
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
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
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
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
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
                    UserList += User + $" [Unmuted in{Days}{Hours}{Minutes}{Seconds}]\n ";
                }
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(UserList).Build());
            }
        }

        [Command("muteinfo")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
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
        [Command("setmuterole")]
        public async Task SetMuteRoleAsync([Remainder] IRole role)
        {
            var Config = Context.Config;
            Config.Moderation.MuteRoleID = role.Id.ToString();
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Mute role has been set to: **{role.Name}**").Build());
        }
        [Command("unmute")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
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
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        public async Task Mute1Async(SocketGuildUser User, int value = 1, string time = "hour", [Remainder] string reason = "No reason has been provided.")
        {
            EventsHandler.Muted = true;
            if (User == Context.User) return;
            var UnmuteTime = Services.MuteService.Extensions.GetTime(value, time);
            var Config = Context.Config;
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
                Config.Moderation.MuteRoleID = Role.Id.ToString();
                Config.Moderation.MuteList.Add(User.Id.ToString(), new MuteWrapper { GuildId = Context.Guild.Id.ToString(), ModeratorId = Context.User.Id.ToString(), MutedAt = DateTime.Now, Reason = reason, UnmuteTime = UnmuteTime});
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
