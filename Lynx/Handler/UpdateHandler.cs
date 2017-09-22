using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Discord;
using Lynx.Database;
using Raven.Client.Documents.Session;

namespace Lynx.Handler
{
    public static class UpdateHandler
    {
        public enum Event
        {
            Logs, LogChannel,
            JoinLog, JoinChannel,
            LeaveLog, LeaveChannel,
            UserBanLog,
            UserUnbanLog,
            PresenceLog,
            StatusPresenceLog,
            VoicePresenceLog,
            MessageUpdateLog,
            MessageDeleteLog,
            ChannelDeleteLog,
            ChannelUpdateLog,
            ChannelCreateLog,
            RoleCreateLog,
            RoleUpdateLog,
            RoleDeleteLog,
            NSFWWarningLog,
        }
        public static async Task ConfigEventsToggle(this IGuild Guild, Event Event, ITextChannel Channel = null)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<SConfig>("GConfigs/" + Guild.Id);
                var Events = Config.Result.Events;
                switch (Event)
                {
                    case Event.LogChannel:
                        Events.LogChannel = Channel.Id.ToString(); break;
                    case Event.Logs:
                        Events.LogState = !Events.LogState; break;
                    case Event.JoinLog:
                        Events.Join.IsEnabled = !Events.Join.IsEnabled; break;
                    case Event.JoinChannel:
                        Events.Join.TextChannel = Channel.Id.ToString(); break;
                    case Event.LeaveLog:
                        Events.Leave.IsEnabled = !Events.Leave.IsEnabled; break;
                    case Event.LeaveChannel:
                        Events.Leave.TextChannel = Channel.Id.ToString(); break;
                    case Event.UserBanLog:
                        Events.UserBan = !Events.UserBan; break;
                    case Event.UserUnbanLog:
                        Events.UserUnban = !Events.UserUnban; break;
                    case Event.StatusPresenceLog:
                        Events.StatusPresenceUpdate = !Events.StatusPresenceUpdate; break;
                    case Event.PresenceLog:
                        Events.PresenceUpdate = !Events.PresenceUpdate; break;
                    case Event.VoicePresenceLog:
                        Events.VoicePresenceUpdate = !Events.VoicePresenceUpdate; break;
                    case Event.MessageUpdateLog:
                        Events.MessageUpdate = !Events.MessageUpdate; break;
                    case Event.MessageDeleteLog:
                        Events.MessageDelete = !Events.MessageDelete; break;
                    case Event.ChannelDeleteLog:
                        Events.ChannelDelete = !Events.ChannelDelete; break;
                    case Event.ChannelUpdateLog:
                        Events.ChannelUpdate = !Events.ChannelUpdate; break;
                    case Event.ChannelCreateLog:
                        Events.ChannelCreate = !Events.ChannelCreate; break;
                    case Event.RoleCreateLog:
                        Events.RoleCreate = !Events.RoleCreate; break;
                    case Event.RoleUpdateLog:
                        Events.RoleUpdate = !Events.RoleUpdate; break;
                    case Event.RoleDeleteLog:
                        Events.RoleDelete = !Events.RoleDelete; break;
                    case Event.NSFWWarningLog:
                        Events.NSFWWarning = !Events.NSFWWarning; break;
                }
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
        public static async Task UpdateServerPrefix(this IGuild Guild, string Prefix)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<SConfig>("GConfigs/" + Guild.Id);
                Config.Result.ServerPrefix = Prefix;
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
        public enum Moderation
        {
            MuteRole,
            AutoAssignRole,
            AutoAssignEvent,
            AddSelfassignableRole,
            RemoveSelfassignableRole,
        }
        public static async Task UpdateServerModeration(this IGuild Guild, Moderation Moderation, ulong RoleId = 0)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<SConfig>("GConfigs/" + Guild.Id);
                var Moderation_ = Config.Result.Moderation;
                switch (Moderation)
                {
                    case Moderation.AutoAssignRole:
                        Moderation_.DefaultAssignRole.AssignRoleID = RoleId.ToString(); break;
                    case Moderation.MuteRole:
                        Moderation_.MuteRoleID = RoleId.ToString(); break;
                    case Moderation.AutoAssignEvent:
                        Moderation_.DefaultAssignRole.AutoAssignEnabled = !Moderation_.DefaultAssignRole.AutoAssignEnabled; break;
                    case Moderation.AddSelfassignableRole:
                        Moderation_.AssignableRoles.Add(RoleId.ToString()); break;
                    case Moderation.RemoveSelfassignableRole:
                        Moderation_.AssignableRoles.Remove(RoleId.ToString()); break;
                }
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
        public enum BotConfig
        {
            BotPrefix,
            BotToken,
            ClarifaiAPIKey,
            Debug
        }
        public static async Task UpdateBotConfig(this IDiscordClient Client, BotConfig BConfig, string InsertValue = null)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<BConfig>("BotConfig");
                var BotConfig_ = Config.Result;
                switch (BConfig)
                {
                    case BotConfig.BotPrefix:
                        BotConfig_.BotPrefix = InsertValue; break;
                    case BotConfig.BotToken:
                        BotConfig_.BotToken = InsertValue; break;
                    case BotConfig.ClarifaiAPIKey:
                        BotConfig_.ClarifaiAPIKey = InsertValue; break;
                    case BotConfig.Debug:
                        BotConfig_.Debug = !BotConfig_.Debug; break;
                }
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
        public enum MuteOption
        {
            Mute,
            Unmute,
        }
        public static async Task UpdateMuteList(this IDiscordClient Client, IUser User, IUser Moderator,MuteOption MuteOption, DateTime UnmuteTime = default(DateTime), string Reason = null)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<GuildMuteList>("GuildMuteList");
                var MuteList = Config.Result.MuteList;
                switch (MuteOption)
                {
                    case MuteOption.Mute:
                        MuteList.Add(User.Id.ToString(), new MuteWrapper { GuildId = (User as SocketGuildUser).Guild.Id.ToString(), MutedAt = DateTime.Now, Reason = Reason, UnmuteTime = UnmuteTime, ModeratorId = Moderator.Id.ToString() }); break;
                    case MuteOption.Unmute:
                        MuteList.Remove(User.Id.ToString()); break;
                }
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
        public static async Task UpdateMuteList(this DiscordSocketClient Client, IUser User, IUser Moderator, MuteOption MuteOption, DateTime UnmuteTime = default(DateTime), string Reason = null)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<GuildMuteList>("GuildMuteList");
                var MuteList = Config.Result.MuteList;
                switch (MuteOption)
                {
                    case MuteOption.Mute:
                        MuteList.Add(User.Id.ToString(), new MuteWrapper { GuildId = (User as SocketGuildUser).Guild.Id.ToString(), MutedAt = DateTime.Now, Reason = Reason, UnmuteTime = UnmuteTime, ModeratorId = Moderator.Id.ToString() }); break;
                    case MuteOption.Unmute:
                        MuteList.Remove(User.Id.ToString()); break;
                }
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
        public enum Message
        {
            LeaveMessage,
            WelcomeMessage,
        }
        public enum Action
        {
            Title,
            Description,
            ThumbnailURL,
            ImageURL,
            Color,
            Author,
            Footer,
            Null,
        }
        public enum EmbedFooterAuthor
        {
            Name,
            IconURL,
        }
        public static async Task UpdateMessages(this IGuild Guild, IUser User, Message Message, Action Action, EmbedFooterAuthor Embed, string Value)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                var Config = Session.LoadAsync<SConfig>("GConfigs/" + Guild.Id);
                var LeaveMessage = Config.Result.LeaveMessage;
                var JoinMessage = Config.Result.WelcomeMessage;
                switch (Message)
                {
                    case Message.LeaveMessage:
                        switch (Action)
                        {
                            case Action.Title:
                                LeaveMessage.Title = Value; break;
                            case Action.Description:
                                LeaveMessage.Description = Value; break;
                            case Action.ThumbnailURL:
                                LeaveMessage.ThumbnailURL = Value; break;
                            case Action.ImageURL:
                                LeaveMessage.ImageURL = Value; break;
                            case Action.Color:
                                LeaveMessage.ColorHex = Convert.ToUInt32(Value); break;
                            case Action.Author:
                                switch (Embed)
                                {
                                    case EmbedFooterAuthor.IconURL:
                                        LeaveMessage.WithAuthor.IconUrl = Value; break;
                                    case EmbedFooterAuthor.Name:
                                        LeaveMessage.WithAuthor.Name = Value; break;
                                }
                                break;
                            case Action.Footer:
                                switch (Embed)
                                {
                                    case EmbedFooterAuthor.IconURL:
                                        LeaveMessage.WithFooter.IconUrl = Value; break;
                                    case EmbedFooterAuthor.Name:
                                        LeaveMessage.WithFooter.Text = Value; break;
                                }
                                break;
                        }
                        break;
                    case Message.WelcomeMessage:
                        switch (Action)
                        {
                            case Action.Title:
                                JoinMessage.Title = Value; break;
                            case Action.Description:
                                JoinMessage.Description = Value; break;
                            case Action.ThumbnailURL:
                                JoinMessage.ThumbnailURL = Value; break;
                            case Action.ImageURL:
                                JoinMessage.ImageURL = Value; break;
                            case Action.Color:
                                JoinMessage.ColorHex = Convert.ToUInt32(Value); break;
                            case Action.Author:
                                switch (Embed)
                                {
                                    case EmbedFooterAuthor.IconURL:
                                        JoinMessage.WithAuthor.IconUrl = Value; break;
                                    case EmbedFooterAuthor.Name:
                                        JoinMessage.WithAuthor.Name = Value; break;
                                }
                                break;
                            case Action.Footer:
                                switch (Embed)
                                {
                                    case EmbedFooterAuthor.IconURL:
                                        JoinMessage.WithFooter.IconUrl = Value; break;
                                    case EmbedFooterAuthor.Name:
                                        JoinMessage.WithFooter.Text = Value; break;
                                }
                                break;
                        }
                        break;
                }
                await Session.StoreAsync(Config);
                await Session.SaveChangesAsync();
                Session.Dispose();
            }
        }
    }
}

        
    

    

    

