using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Lynx.Database;
using System.Threading.Tasks;
using Raven.Client;
using Lynx.Services.Embed;
using System.Drawing;
using System.Globalization;
using Lynx.Handler;
using Raven.Client.Documents.Session;

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Settings : ModuleBase
    {
        [Command("settings")]
        public async Task GuildSettingsAsync()
        {
            await Context.Channel.SendMessageAsync("", embed: EmbedMethods.ShowSettingsEmbed(Context.Guild as SocketGuild).Build());
        }
        [Command("prefix")]
        public async Task ChangeGuildPrefix(string Prefix)
        {
            await Context.Guild.UpdateServerPrefix(Prefix);
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
    }
    [Group("joinsettings")]
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class JoinSettings : ModuleBase
    {
        [Command("thumbnail")]
        public async Task JoinThumbnail(string Url = null)
        {
            await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.ThumbnailURL, UpdateHandler.EmbedFooterAuthor.IconURL, Url);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Thumbnail set to the current server picture.**").Build());
        }
        [Command("title")]
        public async Task JoinTitle([Remainder] string Title = null)
        {
            await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Title, UpdateHandler.EmbedFooterAuthor.IconURL, Title);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Title set to:** " +Title).Build());
        }
        [Command("description")]
        public async Task JoinDescription([Remainder] string Description = null)
        {
            await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Description, UpdateHandler.EmbedFooterAuthor.IconURL, Description);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Description set to:** " + Description).Build());
        }
        [Command("imageurl")]
        public async Task JoinImage(string Url = null)
        {
            await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.ImageURL, UpdateHandler.EmbedFooterAuthor.IconURL, Url);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Image set to the current server picture.**").Build());
        }
        [Command("color")]
        public async Task JoinColor(string HexColor)
        {
            uint argb = UInt32.Parse(HexColor.Replace("#", ""), NumberStyles.HexNumber);
            await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Color, UpdateHandler.EmbedFooterAuthor.IconURL, argb.ToString());
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Embed Color set to:** " + HexColor).Build());
        }
        [Group("author")]
        public class Author : ModuleBase
        {
            [Command("name")]
            public async Task AuthorWithName([Remainder] string Name)
            {
                await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Author, UpdateHandler.EmbedFooterAuthor.Name, Name);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Author Name set to:** " + Name).Build());
            }
            [Command("icon")]
            public async Task AuthorWithIcon(string Url)
            {
                await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Author, UpdateHandler.EmbedFooterAuthor.IconURL, Url);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Author Icon set to:** " + Url).Build());
            }
        }
        [Group("footer")]
        public class Footer : ModuleBase
        {
            [Command("name")]
            public async Task FooterWithName([Remainder] string Name)
            {
                await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Footer, UpdateHandler.EmbedFooterAuthor.Name, Name);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Footer Name set to:** " + Name).Build());
            }
            [Command("icon")]
            public async Task FooterWithIcon(string Url)
            {

                await Context.Guild.UpdateMessages(null, UpdateHandler.Message.WelcomeMessage, UpdateHandler.Action.Footer, UpdateHandler.EmbedFooterAuthor.IconURL, Url);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("**Join Message Footer Icon set to:** " + Url).Build());
            }
        }
    }
    [Group("Log")]
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Events : ModuleBase
    {
        [Command()]
        public async Task ToggleLog(IChannel channel = null)
        {
            if (channel == null)
                channel = Context.Channel;
            var Config = Context.Guild.LoadServerConfig();
            var State = Config.Events.LogState == false ? $"**Started** logging in {(channel as SocketTextChannel).Mention}" : "**Stopped** logging all events.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.Logs);
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.LogChannel, Context.Channel as ITextChannel);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(State).Build());
        }
        [Command("join")]
        public async Task JoinToggle()
        {
             var Config = Context.Guild.LoadServerConfig().Events.Join.IsEnabled;
             var State = Config == false ? "**Started** logging **Join Logs**." : "**Stopped** logging **Join Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.JoinLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("leave")]
        public async Task LeaveToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.Leave.IsEnabled;
            var State = Config == false ? "**Started** logging **Leave Logs**." : "**Stopped** logging **Leave Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.LeaveLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }                  
        [Command("presenceupdate")]
        public async Task PresenceUpdatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.PresenceUpdate;
            var State = Config == false ? "**Started** logging **Presence Update Logs**." : "**Stopped** logging **Presence Update Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.PresenceLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("statuspresenceupdate")]
        public async Task StatusPresenceUpdatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.PresenceUpdate;
            var State = Config == false ? "**Started** logging **Status Presence Update Logs**." : "**Stopped** logging **Status Presence Update Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.StatusPresenceLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("messagedeleted")]
        public async Task MessageDeletedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.MessageDelete;
            var State = Config == false ? "**Started** logging **Message Deleted Logs**." : "**Stopped** logging **Message Deleted Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.MessageDeleteLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("messageupdated")]
        public async Task MessageUpdatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.MessageUpdate;
            var State = Config == false ? "**Started** logging **Message Updated Logs**." : "**Stopped** logging **Message Updated Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.MessageUpdateLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("channelupdated")]
        public async Task ChannelUpdatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.ChannelUpdate;
            var State = Config == false ? "**Started** logging **Channel Updated Logs**." : "**Stopped** logging **Channel Updated Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.ChannelUpdateLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("channelcreated")]
        public async Task ChannelCreatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.ChannelCreate;
            var State = Config == false ? "**Started** logging **Channel Created Logs**." : "**Stopped** logging **Channel Created Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.ChannelCreateLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("channeldeleted")]
        public async Task ChannelDeletedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.ChannelDelete;
            var State = Config == false ? "**Started** logging **Channel Deleted Logs**." : "**Stopped** logging **Channel Deleted Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.ChannelDeleteLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("rolecreated")]
        public async Task RoleCreatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.RoleCreate;
            var State = Config == false ? "**Started** logging **Role Created Logs**." : "**Stopped** logging **Role Created Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.RoleCreateLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("roledeleted")]
        public async Task RoleDeletedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.RoleDelete;
            var State = Config == false ? "**Started** logging **Role Deleted Logs**." : "**Stopped** logging **Role Deleted Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.RoleDeleteLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
        [Command("roleupdated")]
        public async Task RoleUpdatedToggle()
        {
            var Config = Context.Guild.LoadServerConfig().Events.RoleUpdate;
            var State = Config == false ? "**Started** logging **Role Updated Logs**." : "**Stopped** logging **Role Updated Logs**.";
            await Context.Guild.ConfigEventsToggle(UpdateHandler.Event.RoleUpdateLog);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(State).WithSuccesColor().Build());
        }
    }
}
