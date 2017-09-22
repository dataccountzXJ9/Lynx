using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using System.Threading.Tasks;
using System.Linq;
using Lynx.Methods;
using Lynx.Handler;

namespace Lynx.Services.Embed
{
    public static class EmbedMethods
    {
        public static EmbedBuilder WithSuccesColor(this EmbedBuilder Embed)
            => Embed.WithColor(Discord.Color.Green);
        public static EmbedBuilder WithFailedColor(this EmbedBuilder Embed)
            => Embed.WithColor(Discord.Color.Red);
        public static EmbedBuilder ShowSettingsEmbed(SocketGuild Guild)
        {
            var embed = new EmbedBuilder();
            try
            {
                var Config = Guild.LoadServerConfig();
                ITextChannel JoinChannel = Guild.GetJoinLogChannel();
                ITextChannel LeaveChannel = Guild.GetLeaveLogChannel();
                ITextChannel LogChnl = Guild.GetLogChannel();
                var LogState = Config.Events.LogState == true ? $"Enabled in {LogChnl.Mention}" : "Disabled";
                var JoinState = Config.Events.Join.IsEnabled == true ? $"Enabled in {JoinChannel.Mention}" : "Disabled";
                var LeaveState = Config.Events.Leave.IsEnabled == true ? $"Enabled in {LeaveChannel.Mention}" : "Disabled";
                var BanState = Config.Events.UserBan & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var UnbanState = Config.Events.UserUnban & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var PresenceState = Config.Events.PresenceUpdate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var StatusPresence = Config.Events.StatusPresenceUpdate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var MSGDeletedState = Config.Events.MessageDelete & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var MSGUpdatedState = Config.Events.MessageUpdate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var CHDeletedState = Config.Events.ChannelDelete & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var CHCreatedState = Config.Events.ChannelCreate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var CHDUpdatedState = Config.Events.ChannelUpdate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var RLCreatedState = Config.Events.RoleCreate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var RLUpdatedState = Config.Events.RoleUpdate & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var RLDeletedState = Config.Events.RoleDelete & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var NSFWState = Config.Events.NSFWWarning & Config.Events.LogState == true ? $"Enabled" : "Disabled";
                var LogChannel = Config.Events.LogChannel != "0" ? (Guild.GetTextChannel(Convert.ToUInt64(Config.Events.LogChannel)) as SocketTextChannel).Mention : "No log channel set.";
                embed.AddField(x =>
                {
                    x.Name = "General";
                    x.Value = "**Prefix:** " + Guild.GetPrefix() + "\n" +
                    $"**Welcome Message:** Type {Guild.GetPrefix()}exwelcome.\n"
                    + $"**Leave Message:** Type {Guild.GetPrefix()}exleave.\n";
                });
                embed.AddField(x =>
                {
                    x.Name = "Events";
                    x.Value = "**Logs:** " + LogState + "\n**Join Logs:** " + JoinState +
                    "\n**Leave Logs:** " + LeaveState + "\n\n" +
                    $"**Ban Logs:** " + BanState + "\n" +
                    $"**Unban Logs: **" + UnbanState + "\n" +
                    $"**Presence Logs: **" + PresenceState + "\n" +
                    $"**Status Presence Logs:** " + StatusPresence + "\n" +
                    $"**Message Deleted Logs: **" + MSGDeletedState + "\n" +
                    $"**Message Updated Logs: **" + MSGUpdatedState + "\n" +
                    $"**Channel Created Logs: **" + CHCreatedState + "\n" +
                    $"**Channel Deleted Logs Logs: **" + CHDeletedState + "\n" +
                    $"**Channel Updated Logs: **" + CHDUpdatedState + "\n" +
                    $"**NSFW Warning Logs: **" + NSFWState + "\n";
                }).WithSuccesColor().WithThumbnailUrl(Guild.IconUrl);
                embed.WithTitle($"Settings for {Guild.Name}");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return embed;
        }
        public static EmbedBuilder GetWelcomeEmbed(ulong ServerId, SocketUser User)
        {
            var WelcomeMessage = "";
            var Config = (User as SocketGuildUser).Guild.LoadServerConfig().WelcomeMessage;
            if (Config.Description == null)
            {
                WelcomeMessage = $"%usermention% joined %servername%";
                WelcomeMessage = Placeholders.GetPlaceholder(WelcomeMessage, User as IUser);
            }
            else
            {
                WelcomeMessage = Config.Description;
                WelcomeMessage = Placeholders.GetPlaceholder(WelcomeMessage, User as IUser);
            }
            return new EmbedBuilder()
            .WithColor(Config.ColorHex)
            .WithTitle(Placeholders.GetPlaceholder(Config.Title, User as IUser))
            .WithDescription(WelcomeMessage)
            .WithImageUrl(Placeholders.GetPlaceholder(Config.ImageURL, User as IUser))
            .WithThumbnailUrl(Placeholders.GetPlaceholder(Config.ThumbnailURL, User as IUser))
            .WithAuthor(x =>
            {
                x.IconUrl = Placeholders.GetPlaceholder(Config.WithAuthor.IconUrl, User as IUser);
                x.Name = Placeholders.GetPlaceholder(Config.WithAuthor.Name, User as IUser);

            })
            .WithFooter(x =>
            {
                x.IconUrl = Placeholders.GetPlaceholder(Config.WithFooter.IconUrl, User as IUser);
                x.Text = Placeholders.GetPlaceholder(Config.WithFooter.Text, User as IUser);
            });
        }

        public static EmbedBuilder GetLeaveEmbed(ulong ServerId, SocketUser User)
        {
            var LeaveMessage = "";
            var Config= (User as SocketGuildUser).Guild.LoadServerConfig().LeaveMessage;
            if (Config.Description == null)
            {
                LeaveMessage = $"%usermention% left %servername%";
                LeaveMessage = Placeholders.GetPlaceholder(LeaveMessage, User as IUser);
            }
            else if (Config.Description != null)
            {
                LeaveMessage = Config.Description;
                LeaveMessage = Placeholders.GetPlaceholder(LeaveMessage, User as IUser);
            }
            return new EmbedBuilder()
                .WithColor(Config.ColorHex)
                .WithTitle(Placeholders.GetPlaceholder(Config.Title, User as IUser))
                .WithDescription(LeaveMessage)
                .WithImageUrl(Placeholders.GetPlaceholder(Config.ImageURL, User as IUser))
                .WithThumbnailUrl(Placeholders.GetPlaceholder(Config.ThumbnailURL, User as IUser))
                .WithAuthor(x =>
                 {
                     x.IconUrl = Config.WithAuthor.IconUrl;
                     x.Name = Placeholders.GetPlaceholder(Config.WithAuthor.Name, User as IUser);

                 })
                .WithFooter(x =>
                {
                    x.IconUrl = Config.WithFooter.IconUrl;
                    x.Text = Placeholders.GetPlaceholder(Config.WithFooter.Text, User as IUser);
                });
        }                       
        public static EmbedBuilder JoinLogEmbed(SocketUser User)
        {
            return new EmbedBuilder()
                        .AddField(x =>
                        {
                            x.Name = @"\✅ User Joined";
                            x.Value = User.Username + "#" + User.Discriminator;
                        })
                        .AddField(x =>
                        {
                            x.Name = "User Id";
                            x.Value = $"{User.Id}";
                        })
                        .WithFooter(x =>
                        {
                            x.Text = $"{DateTime.Now.ToString(@"hh\:mm\:ss")}";
                        })
                        .WithSuccesColor()
                        .WithThumbnailUrl(User.GetAvatarUrl());
        }
        public static EmbedBuilder LeaveLogEmbed(SocketUser User)
        {
            return new EmbedBuilder()
            .AddField(x =>
            {
                x.Name = @"\❌ User Left";
                x.Value = User.Username + "#" + User.Discriminator;
            })
            .AddField(x =>
            {
                x.Name = "User Id";
                x.Value = $"{User.Id}";
            })
            .WithFooter(x =>
            {
                x.Text = $"{DateTime.Now.ToString(@"hh\:mm\:ss")}";
            })
            .WithSuccesColor()
            .WithThumbnailUrl(User.GetAvatarUrl());
        }
        public static EmbedBuilder BanLogEmbed(SocketUser User)
        {
            return new EmbedBuilder()
            .AddField(x =>
            {
                x.Name = @"\🚫 User Banned";
                x.Value = User.Username + "#" + User.Discriminator;
            })
            .AddField(x =>
            {
                x.Name = "User Id";
                x.Value = $"{User.Id}";
            })
            .WithFooter(x =>
            {
                x.Text = $"{DateTime.Now.ToString(@"hh\:mm\:ss")}";
            })
            .WithSuccesColor()
            .WithThumbnailUrl(User.GetAvatarUrl());
        }
        public static EmbedBuilder UnbanLogEmbed(SocketUser User)
        {
            return new EmbedBuilder()
           .AddField(x =>
           {
               x.Name = @"\♻️ User Unbanned";
               x.Value = User.Username + "#" + User.Discriminator;
           })
          .AddField(x =>
           {
               x.Name = "User Id";
               x.Value = $"{User.Id}";
           })
           .WithFooter(x =>
           {
               x.Text = $"{DateTime.Now.ToString(@"hh\:mm\:ss")}";
           })
           .WithSuccesColor()
           .WithThumbnailUrl(User.GetAvatarUrl());
        }
        public static EmbedBuilder PresenceLogEmbed(SocketUser OUser, SocketUser UUser,Presence Presence,IEnumerable<string> Roles = null)
        {
            var e = new EmbedBuilder();
            switch (Presence)
            {
                case Presence.Status:
                    e.WithTitle("Presence Updates");
                    e.WithDescription($@"🎭 `【{DateTime.Now.ToString(@"hh\:mm\:ss")}】` **{OUser.Username}#{OUser.Discriminator}** is now **{UUser.Status}**");
                    e.WithSuccesColor(); break;
                case Presence.Username:
                    e.WithTitle("Presence Updates");
                    e.WithDescription($@"🎭 `【{DateTime.Now.ToString(@"hh\:mm\:ss")}】` **{OUser.Username}#{OUser.Discriminator}** changed username to **{UUser.Username}#{UUser.Discriminator}**");
                    e.WithSuccesColor(); break;
                case Presence.Nickname:
                    e.WithTitle("Guild User Presence Updates");
                    e.WithDescription($@"🎭 `【{DateTime.Now.ToString(@"hh\:mm\:ss")}】` **{OUser.Username}#{OUser.Discriminator}** changed nickname to **{(UUser as SocketGuildUser).Nickname ?? UUser.Username}** from **{(OUser as SocketGuildUser).Nickname ?? UUser.Username}**.");
                    e.WithSuccesColor(); break;
                case Presence.Avatar:
                    e.WithTitle("Presence Updates");
                    e.WithDescription($@"🎭 `【{DateTime.Now.ToString(@"hh\:mm\:ss")}】` **{OUser.Username}#{OUser.Discriminator}** changed avatar."); 
                    e.WithThumbnailUrl(UUser.GetAvatarUrl());
                    e.WithSuccesColor(); break;
                case Presence.Discriminator:
                    e.WithTitle("Presence Updates");
                    e.WithDescription($@"🎭 `【{DateTime.Now.ToString(@"hh\:mm\:ss")}】` **{OUser.Username}#{OUser.Discriminator}** discriminator to **{UUser.Username}#{UUser.Discriminator}**.");
                    e.WithSuccesColor(); break;
                case Presence.Game:
                    e.WithTitle("Presence Updates");
                    e.WithDescription($@"🎭 `【{DateTime.Now.ToString(@"hh\:mm\:ss")}】` **{OUser.Username}#{OUser.Discriminator}** is now playing **{UUser.Game?.Name ?? "-"}**");
                    e.WithSuccesColor(); break;
                case Presence.UserRoleAdded:
                    e.WithTitle("Guild User Presence Updates");
                    e.WithDescription($@"\🛡 The following role(s) have been added to {UUser}." + $"\n```css\n" +
                        $"{string.Join(", ", Roles)}```");
                    e.WithSuccesColor(); break;
                case Presence.UserRoleRemoved:
                    e.WithTitle("Guild User Presence Updates");
                    e.WithDescription($@"\🛡 The following role(s) have been removed from {UUser}." + $"\n```css\n" +
                        $"{string.Join(", ", Roles)}```");
                    e.WithSuccesColor(); break;

            }
            return e;
        }      
        public static EmbedBuilder MessageUpdatedEmbed(Cacheable<IMessage, ulong> Cache, SocketMessage Message)
        {
            return new EmbedBuilder().WithTitle($@"📝 Message updated in #{Message.Channel.Name}")
                    .AddField(x =>
                    {
                        x.Name = "Old Message";
                        x.Value = $"`{Message.Author.Username}#{Message.Author.Discriminator}:` {Cache.Value.Content}";
                    })
                    .AddField(x =>
                    {
                        x.Name = "New Message";
                        x.Value = $"`{Message.Author.Username}#{Message.Author.Discriminator}:` {Message.Content}";
                    }).WithSuccesColor();
        }
        public static EmbedBuilder MessageDeletedEmbed(Cacheable<IMessage, ulong> Cache, IUserMessage Message)
        {
            var Attachments = Cache.Value.Embeds;
            var Attachment = Attachments.Skip(1).Select(x => x.Url).ToString();
            var embed = new EmbedBuilder();
            embed.Title = $"🗑 Message deleted in #{Message.Channel.Name}";
            embed.WithSuccesColor();
            embed.AddField(x =>
            {
                x.Name = "Message Content";
                x.Value = $"{Cache.Value.Content}";
            });
            embed.AddField(x =>
            {
                x.Name = "Message Author";
                x.Value = Cache.Value.Author;
            });
            if (Cache.Value.Attachments.Any())
            {
                embed.AddField(x =>
                {
                    x.Name = "Message Attachments";
                    x.Value = string.Join(", ", Cache.Value.Attachments.Select(z => z.Url));
                }).WithSuccesColor();
            }
            return embed.WithSuccesColor();
        }
        public static EmbedBuilder ChannelUpdatedEmbed(SocketChannel OChannel, SocketChannel UChannel, Channel Channel)
        {
            var e = new EmbedBuilder();

            switch(Channel)
            {
                case Channel.Name:
                    e.WithTitle("📝 Channel name updated").AddField(x =>
                    {
                        x.Name = "Previous name";
                        x.Value = (OChannel as SocketTextChannel).Name;
                    }).AddField(x =>
                    {
                        x.Name = "Updated name";
                        x.Value = (UChannel as SocketTextChannel).Name;
                    }).WithSuccesColor(); break;
                case Channel.Topic:
                    e.WithTitle("📝 Channel topic updated").AddField(x =>
                    {
                        x.Name = "Previous topic";
                        x.Value = (OChannel as SocketTextChannel).Topic ?? "No topic set.";
                    }).AddField(x =>
                    {
                        x.Name = "Updated topic";
                        x.Value = (UChannel as SocketTextChannel).Topic ?? "No topic set.";
                    }).WithSuccesColor(); break;
            }
            return e; 
        }
        public enum Presence
        {
            Avatar,
            Nickname,
            Status,
            Username,
            Discriminator,
            Game,
            UserRoleAdded,
            UserRoleRemoved,
        }
        public enum Channel
        {
            Name,
            Topic,
        }
    }
}
       
    

        
    

    

    
    
    

