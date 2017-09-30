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

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Settings : LynxBase<LynxContext>
    {
        static GuildConfig GuildConfig = new GuildConfig();
        [Command("autoassign")]
        public async Task GuildSettingsAsync([Remainder] IRole role = null)
        {
            var Config = Context.Config;
            var Moderation = Config.Moderation.DefaultAssignRole;
            if (role != null)
                if ((Context.User as SocketGuildUser).Roles.Max(x => x.Position) <= role.Position)
                    return;
            if (role == null && Moderation.AutoAssignEnabled == true)
            {
                Moderation.AutoAssignEnabled = false;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("Succesfully **disabled** auto-assign service. I will no longer auto-assign roles to new users.").WithFooter(x => x.WithText($"{Context.Guild.GetPrefix()}autoassign <role_name> to enable.").WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl())).Build());
                return;
            }
            else if (role == null && Moderation.AutoAssignEnabled == false)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Auto-assigning is already **disabled**.").WithFooter(x => x.WithText($"{Context.Guild.GetPrefix()}autoassign <role_name> to enable.").WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl())).Build());
                return;
            }
            else 
            {
                Moderation.AssignRoleID = role.Id.ToString();
                Moderation.AutoAssignEnabled = true;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"I will now assign **{role.Name}** to new users!").WithFooter(x => x.WithText($"{Context.Guild.GetPrefix()}autoassign to disable.").WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl())).Build());

            }
        }
        [Command("settings")]
        public async Task GuildSettingsAsync()
        {
            await Context.Channel.SendMessageAsync("", embed: EmbedMethods.ShowSettingsEmbed(Context.Guild as SocketGuild).Build());
        }
        [Command("prefix")]
        public async Task ChangeGuildPrefix(string Prefix)
        {
            var Config = Context.Config;
            Config.ServerPrefix = Prefix;
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Guild Prefix has been changed to:** " + Prefix).Build());
        }
        [Command("exleave")]
        public async Task ExampleLeaveMessage()
        {
            await Context.Channel.SendMessageAsync("", embed: EmbedMethods.GetLeaveEmbed(Context.Guild.Id, Context.User as SocketUser).Build());
        }
        [Command("exwelcome")]
        public async Task ExampleJoinMessage()
        {
            await Context.Channel.SendMessageAsync("", embed: EmbedMethods.GetWelcomeEmbed(Context.Guild.Id, Context.User as SocketUser).Build());
        }

        [Group("Joinsettings")]
        [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        public class JoinSettings : ModuleBase<LynxContext>
        {
            [Command("thumbnail")]
            public async Task JoinThumbnail(string Url = null)
            {
                var Config = Context.Config;
                Config.WelcomeMessage.ThumbnailURL = Url;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Thumbnail set to the current server picture.**").Build());
            }
            [Command("title")]
            public async Task JoinTitle([Remainder] string Title = null)
            {
                var Config = Context.Config;
                Config.WelcomeMessage.Title = Title;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Title set to:** " + Title).Build());
            }
            [Command("description")]
            public async Task JoinDescription([Remainder] string Description = null)
            {
                var Config = Context.Config;
                Config.WelcomeMessage.Description = Description;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Description set to:** " + Description).Build());
            }
            [Command("imageurl")]
            public async Task JoinImage(string Url = null)
            {
                var Config = Context.Config;
                Config.WelcomeMessage.ImageURL = Url;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Image set to the current server picture.**").Build());
            }
            [Command("color")]
            public async Task JoinColor(string HexColor)
            {
                var Config = Context.Config;
                uint argb = UInt32.Parse(HexColor.Replace("#", ""), NumberStyles.HexNumber);
                Config.WelcomeMessage.ColorHex = argb;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Embed Color set to:** " + HexColor).Build());
            }
            [Group("author")]
            public class Author : ModuleBase<LynxContext>
            {
                [Command("name")]
                public async Task AuthorWithName([Remainder] string Name)
                {
                    var Config = Context.Config;
                    Config.WelcomeMessage.WithAuthor.Name = Name;
                    await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Author Name set to:** " + Name).Build());
                }
                [Command("icon")]
                public async Task AuthorWithIcon(string Url)
                {
                    var Config = Context.Config;
                    Config.WelcomeMessage.WithAuthor.IconUrl = Url;
                    await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Author Icon set to:** " + Url).Build());
                }
            }
            [Group("footer")]
            public class Footer : ModuleBase<LynxContext>
            {
                [Command("name")]
                public async Task FooterWithName([Remainder] string Name)
                {
                    var Config = Context.Config;
                    Config.WelcomeMessage.WithFooter.Text = Name;
                    await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Footer Name set to:** " + Name).Build());
                }
                [Command("icon")]
                public async Task FooterWithIcon(string Url)
                {

                    var Config = Context.Config;
                    Config.WelcomeMessage.WithFooter.IconUrl = Url;
                    await GuildConfig.SaveAsync(Config, Context.Guild.Id); 
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Footer Icon set to:** " + Url).Build());
                }
            }
        }
        [Group("Log")]
        [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        public class Events : ModuleBase<LynxContext>
        {
            [Command()]
            public async Task ToggleLog(IChannel channel = null)
            {
                if (channel == null)
                    channel = Context.Channel;
                var Config = Context.Config;
                var State = Config.Events.LogState == false ? $"**Started** logging in {(channel as SocketTextChannel).Mention}" : "**Stopped** logging all events.";
                Config.Events.LogState = !Config.Events.LogState;
                Config.Events.LogChannel = channel.Id.ToString() ?? Context.Channel.Id.ToString();
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(State).Build());
            }
            [Command("join")]
            public async Task JoinToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.Join.IsEnabled == false ? "**Started** logging **Join Logs**." : "**Stopped** logging **Join Logs**.";
                Config.Events.Join.IsEnabled = !Config.Events.Join.IsEnabled;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("leave")]
            public async Task LeaveToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.Leave.IsEnabled == false ? "**Started** logging **Leave Logs**." : "**Stopped** logging **Leave Logs**.";
                Config.Events.Leave.IsEnabled = !Config.Events.Leave.IsEnabled;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("presenceupdate")]
            public async Task PresenceUpdatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.PresenceUpdate == false ? "**Started** logging **Presence Update Logs**." : "**Stopped** logging **Presence Update Logs**.";
                Config.Events.PresenceUpdate = !Config.Events.PresenceUpdate;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("statuspresenceupdate")]
            public async Task StatusPresenceUpdatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.StatusPresenceUpdate == false ? "**Started** logging **Status Presence Update Logs**." : "**Stopped** logging **Status Presence Update Logs**.";
                Config.Events.StatusPresenceUpdate = !Config.Events.StatusPresenceUpdate;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("messagedeleted")]
            public async Task MessageDeletedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.MessageDelete == false ? "**Started** logging **Message Deleted Logs**." : "**Stopped** logging **Message Deleted Logs**.";
                Config.Events.MessageDelete = !Config.Events.MessageDelete;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("messageupdated")]
            public async Task MessageUpdatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.MessageUpdate == false ? "**Started** logging **Message Updated Logs**." : "**Stopped** logging **Message Updated Logs**.";
                Config.Events.MessageUpdate = !Config.Events.MessageUpdate;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("channelupdated")]
            public async Task ChannelUpdatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.ChannelUpdate == false ? "**Started** logging **Channel Updated Logs**." : "**Stopped** logging **Channel Updated Logs**.";
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("channelcreated")]
            public async Task ChannelCreatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.ChannelCreate == false ? "**Started** logging **Channel Created Logs**." : "**Stopped** logging **Channel Created Logs**.";
                Config.Events.ChannelCreate = !Config.Events.ChannelCreate;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("channeldeleted")]
            public async Task ChannelDeletedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.ChannelDelete == false ? "**Started** logging **Channel Deleted Logs**." : "**Stopped** logging **Channel Deleted Logs**.";
                Config.Events.ChannelDelete = !Config.Events.ChannelDelete;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("rolecreated")]
            public async Task RoleCreatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.RoleCreate == false ? "**Started** logging **Role Created Logs**." : "**Stopped** logging **Role Created Logs**.";
                Config.Events.RoleCreate = !Config.Events.RoleCreate;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);

                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("roledeleted")]
            public async Task RoleDeletedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.RoleDelete == false ? "**Started** logging **Role Deleted Logs**." : "**Stopped** logging **Role Deleted Logs**.";
                Config.Events.RoleDelete= !Config.Events.RoleDelete;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
            [Command("roleupdated")]
            public async Task RoleUpdatedToggle()
            {
                var Config = Context.Config;
                var State = Config.Events.RoleUpdate == false ? "**Started** logging **Role Updated Logs**." : "**Stopped** logging **Role Updated Logs**.";
                Config.Events.RoleUpdate = !Config.Events.RoleUpdate;
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
            }
        }
    }
}
