using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Methods;
using Lynx.Services.Embed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lynx.Services.Currency;
using NLog;
using Octokit;
namespace Lynx.Handler
{
    public class EventsHandler
    {
        IServiceProvider provider;
        CommandService commands;
        static Timer Timer;
        public static MuteHandler Mute = new MuteHandler();
        static DiscordSocketClient Client;
        static GuildConfig GuildConfig = new GuildConfig();
        static LynxConfig LynxConfig = new LynxConfig();
        public static NSFW.NSFWService NSFWService = new NSFW.NSFWService();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static bool Banned = false;
        public static bool Kicked = false;
        public static bool Unmuted = false;
        public static bool Muted = false;
        public static bool Deleted = false;
        public static bool NSFWDeleted = false;
        public EventsHandler(IServiceProvider Prov)
        {
            provider = Prov;
            Client = provider.GetService<DiscordSocketClient>();
            commands = provider.GetService<CommandService>();
            Client.GuildAvailable += async (Guild) =>
            {
                var _ = Task.Run(async () =>
                {
                    foreach (var User in Guild.Users)
                    {
                        if (GuildConfig.LoadAsync(Guild.Id).Currency.UsersList.ContainsKey(User.Id.ToString()) == false)
                        {
                            logger.Info($"{User} has been added to the {Guild.Name}'s currency list.");
                            await User.AddToCurrencyList();
                        }
                    }
                });
                await GuildConfig.LoadOrDeleteAsync(Database.Enums.Actions.Add, Guild.Id);
            };
            Client.JoinedGuild += async (Guild) =>
            {
                await GuildConfig.LoadOrDeleteAsync(Database.Enums.Actions.Add, Guild.Id);
            };
            Client.LeftGuild += async (Guild) =>
            {
                await GuildConfig.LoadOrDeleteAsync(Database.Enums.Actions.Delete, Guild.Id);
            };
            Client.UserJoined += OnUserJoin;
            Client.UserLeft += OnUserLeft;
            Client.UserBanned += async (User, Guild) => { await OnUserBanned(User, Guild); Banned = true; };
            Client.UserUnbanned += OnUserUnbanned;
            Client.GuildMemberUpdated += OnGuildPresenceUpdated;
            Client.UserUpdated += OnPresenceUpdated;
            Client.MessageUpdated += OnMessageUpdated;
            Client.MessageDeleted += OnMessageDeleted;
            Client.ChannelUpdated += OnChannelUpdated;
            Client.ChannelDestroyed += OnChannelDeleted;
            Client.ChannelCreated += OnChannelCreated;
            Client.RoleCreated += OnRoleCreated;
            Client.RoleDeleted += OnRoleDeleted;
    //      Client.RoleUpdated += OnRoleUpdated; 
        }
        internal static void StartStatusService(DiscordSocketClient Client)
        {
            Timer = new Timer(_ =>
            {
                Task.Run(async () =>
                {
                    var Config = LynxConfig.LoadConfig;
                    if (Config.BotGames.Count == 0)
                        return;
                        var RNG = new Random();
                        var GameToPlay = RNG.Next(0, Config.BotGames.Count);
                        await Client.SetGameAsync(Config.BotGames[GameToPlay]);
                });
            }, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }
        internal async Task OnUserJoin(SocketUser User)
        {
            await User.AddToCurrencyList();
            var Guild = (User as SocketGuildUser).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if(Config.Moderation.DefaultAssignRole.AssignRoleID != "0" && Config.Moderation.DefaultAssignRole.AutoAssignEnabled == true)
            {
                await (User as SocketGuildUser).AddRoleAsync(Guild.GetRole(Convert.ToUInt64(Config.Moderation.DefaultAssignRole.AssignRoleID)) as IRole);
            }
            if (Config.Events.Join.IsEnabled == true && Config.Events.LogChannel == "0")
            {
                var Channel = Guild.GetLogChannel();
                await Channel.SendMessageAsync("", embed: EmbedMethods.JoinLogEmbed(User).Build());
            }
            if (Config.Events.LogState == true && Config.Events.Join.IsEnabled== true && Config.Events.Join.TextChannel != "0" && Config.Events.LogChannel != "0")
            {
                var LChannel = Guild.GetJoinLogChannel();
                await LChannel.SendMessageAsync("", embed: EmbedMethods.GetWelcomeEmbed(Guild.Id, User).Build());
                var Channel = Guild.GetLogChannel();
                await Channel.SendMessageAsync("", embed: EmbedMethods.JoinLogEmbed(User).Build());

            }
        }
        internal async Task OnUserLeft(SocketUser User)
        {
            // not removing from currency list because they always could come back!
            if (Banned == true)
            {
                Banned = false;
                return;
            }
            if (Banned == false)
            {
                var Guild = (User as SocketGuildUser).Guild;
                var Config = GuildConfig.LoadAsync(Guild.Id);
                if (Config.Events.LogState == true && Config.Events.LogChannel == "0")
                {
                    var LChannel = Guild.GetLogChannel();
                    await LChannel.SendMessageAsync("", embed: EmbedMethods.LeaveLogEmbed(User).Build());
                }
                if (Config.Events.Leave.IsEnabled == true && Config.Events.LogChannel != "0")
                {
                    var Channel = Guild.GetLeaveLogChannel();
                    await Channel.SendMessageAsync("", embed: EmbedMethods.GetLeaveEmbed(Guild.Id, User).Build());
                    var LChannel = Guild.GetLogChannel();
                    await LChannel.SendMessageAsync("", embed: EmbedMethods.LeaveLogEmbed(User).Build());
                }
            }
        }
        internal async Task OnUserBanned(SocketUser User, SocketGuild Guild)
        {
            await User.RemoveFromCurrencyList();
            Banned = true;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.UserBan == true && Config.Events.LogChannel != "0")
            {
                await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.BanLogEmbed(User).Build());
            }
        }
        internal async Task OnUserUnbanned(SocketUser User, SocketGuild Guild)
        {
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.UserUnban == true && Config.Events.LogChannel != "0")
            {
                await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.UnbanLogEmbed(User).Build());
            }
        }
        internal async Task OnGuildPresenceUpdated(SocketUser OutdatedUser, SocketUser UpdatedUser)
        {
            var UUser = (UpdatedUser as SocketGuildUser);
            var OUser = (OutdatedUser as SocketGuildUser);
            var Config = GuildConfig.LoadAsync(UUser.Guild.Id);
            if (Config.Events.LogState == true && Config.Events.PresenceUpdate == true && Config.Events.LogChannel != "0")
            {
                var Guild = UUser.Guild;
                if (OUser.Status != UUser.Status && Config.Events.StatusPresenceUpdate == true)
                {
                    await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.Status).Build());
                }
                else if (OUser.Nickname != UUser.Nickname)
                {
                    await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.Nickname).Build());
                }
                else if (OUser.Game?.Name != UUser.Game?.Name && UUser.IsBot == false)
                {
             //       await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.Game).Build());
                }
                else if (!OUser.Roles.SequenceEqual(UUser.Roles))
                {
                    if (OUser.Roles.Count < UUser.Roles.Count)
                    {
                        if (Muted == true)
                        {
                            Muted = false;
                            return;
                        }
                            var diffRoles = UUser.Roles.Where(r => !OUser.Roles.Contains(r)).Select(r => r.Name);
                        await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.UserRoleAdded, diffRoles).Build());
                    }
                    else if (OUser.Roles.Count > UUser.Roles.Count)
                    {
                        if (Unmuted == true)
                        {
                            Unmuted = false;
                            return;
                        }
                        var diffRoles = OUser.Roles.Where(r => !UUser.Roles.Contains(r)).Select(r => r.Name);
                        var MuteList = Config.Moderation.MuteList.TryGetValue(UUser.Id.ToString(), out MuteWrapper Value);
                        if (diffRoles.Contains("Lynx-Mute") && Value.UnmuteTime >= DateTime.Now)
                        {
                            Config.Moderation.MuteList.Remove(UUser.Id.ToString());
                            await GuildConfig.SaveAsync(Config, Guild.Id);
                                 await Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithAuthor(x=> { x.Name = "User force unmuted.";x.IconUrl = UUser.GetAvatarUrl(); })
                                .WithDescription($"**{UUser}** has been force unmuted.\n\n**Unmute at:** {DateTime.Now}\n\n").AddField(x=> { x.Name = "Mute Report:";
                                    x.Value = $"**Muted at:** {Value.MutedAt}\n" +
                                        $"**Muted until:** {Value.UnmuteTime}\n" +
                                        $"**Muted by:** {(Guild.GetUser(Convert.ToUInt64(Value.ModeratorId))as IUser)}\n**Reason:** {Value.Reason}"; }).Build());
                            return;
                        }
                        await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.UserRoleRemoved, diffRoles).Build());
                    }
                }
            }
        }
        
            
        
        internal async Task OnPresenceUpdated(SocketUser OutdatedUser, SocketUser UpdatedUser)
        {
            var UUser = (UpdatedUser as SocketGuildUser);
            var OUser = (OutdatedUser as SocketGuildUser);
            var Config = GuildConfig.LoadAsync(OUser.Guild.Id);
            if (Config.Events.LogState == true && Config.Events.PresenceUpdate == true && Config.Events.LogChannel != "0")
            {
                var Guild = UUser.Guild;
                if (OutdatedUser.Username != UpdatedUser.Username)
                {
                    await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OutdatedUser, UpdatedUser, EmbedMethods.Presence.Username).Build());
                }
                else if (OutdatedUser.AvatarId != UpdatedUser.AvatarId)
                {
                    await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OutdatedUser, UpdatedUser, EmbedMethods.Presence.Avatar).Build());
                }
                else if (OutdatedUser.Discriminator != UpdatedUser.Discriminator)
                {
                    await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OutdatedUser, UpdatedUser, EmbedMethods.Presence.Discriminator).Build());
                }
            }
        }
        
        internal async Task OnMessageUpdated(Cacheable<IMessage, ulong> Cache, SocketMessage Message, ISocketMessageChannel Channel)
        {
            if (Message.Author == Client.CurrentUser || Message.Author.IsBot == true) return;
            var Before = (Cache.HasValue ? Cache.Value : null) as IUserMessage;
            if (Before == null)
                return;
            if (Before.Content == Message.Content)
                return;
            var Guild = (Message.Author as SocketGuildUser).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.MessageUpdate == true && Config.Events.LogChannel != "0")
            {
                await (Channel as SocketTextChannel).Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.MessageUpdatedEmbed(Cache, Message).Build());
            }
        }
        internal async Task OnMessageDeleted(Cacheable<IMessage, ulong> Cache, ISocketMessageChannel Channel)
        {
            if (Deleted == true)
            {
                Deleted = false;
                return;
            }
            if (NSFWDeleted == true)
            {
                NSFWDeleted = false;
                return;
            }
            if (Cache.Value.Author == Client.CurrentUser || Cache.Value.Author.IsBot == true) return;
            var Before = (Cache.HasValue ? Cache.Value : null) as IUserMessage;

            var Guild = (Cache.Value.Channel as SocketTextChannel).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.MessageDelete == true && Config.Events.LogChannel != "0")
            {
                await (Channel as SocketTextChannel).Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.MessageDeletedEmbed(Cache, Before).Build());
            }
        }
        internal async Task OnChannelUpdated(SocketChannel OutdatedChannel, SocketChannel UpdatedChannel)
        {
            var Guild = (UpdatedChannel as SocketTextChannel).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState== true && Config.Events.ChannelUpdate== true && Config.Events.LogChannel != "0")
            {
                if((OutdatedChannel as SocketTextChannel).Name != (UpdatedChannel as SocketTextChannel).Name)
                {
                    await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.ChannelUpdatedEmbed(OutdatedChannel, UpdatedChannel, EmbedMethods.Channel.Name).Build());
                }
                else if((OutdatedChannel as SocketTextChannel)?.Topic != (UpdatedChannel as SocketTextChannel).Topic)
                {
                    await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.ChannelUpdatedEmbed(OutdatedChannel, UpdatedChannel, EmbedMethods.Channel.Topic).Build());
                }
            }
        }
        internal async Task OnChannelDeleted(SocketChannel Channel)
        {
            var Guild = (Channel as SocketTextChannel).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.ChannelDelete == true && Config.Events.LogChannel != "0")
            {
                var embed = new EmbedBuilder();
                embed.WithTitle($"🗑 #{(Channel as SocketTextChannel).Name} has been deleted").AddField(x=>
                {
                    x.Name = "Channel Name";
                    x.Value = (Channel as SocketTextChannel).Name + $" [{Channel.Id}]";
                }).WithSuccesColor();
                await Guild.GetLogChannel().SendMessageAsync("", embed: embed.Build());
            }

        }
        internal async Task OnChannelCreated(SocketChannel Channel)
        {
            var Guild = (Channel as SocketTextChannel).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.ChannelCreate == true && Config.Events.LogChannel != "0")
            {
                var embed = new EmbedBuilder();
                embed.WithTitle($"🗑 #{(Channel as SocketTextChannel).Name} has been created").AddField(x =>
                {
                    x.Name = "Channel Name";
                    x.Value = (Channel as SocketTextChannel).Name + $" [{Channel.Id}]";
                }).WithSuccesColor();
                await Guild.GetLogChannel().SendMessageAsync("", embed: embed.Build());
            }
        }
        internal async Task OnRoleCreated(IRole Role)
        {
            var Guild = (Role as SocketRole).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogState == true && Config.Events.ChannelCreate == true && Config.Events.LogChannel != "0")
            {
                var embed = new EmbedBuilder();
                embed.WithTitle($"🛡 {Role.Name} has been created").AddField(x =>
                {
                    x.Name = "Role Name";
                    x.Value = Role.Name;
                }).AddField(x =>
                {
                    x.Name = "Role Permissions";
                    x.Value = string.Join("\n", Role.Permissions);
                }).WithSuccesColor();
                await Guild.GetLogChannel().SendMessageAsync("", embed: embed.Build());
            }
        }
        internal async Task OnRoleDeleted(SocketRole role)
        {
            var Guild = (role as SocketRole).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (role.Id.ToString() == Config.Moderation.MuteRoleID)
            {
                Config.Moderation.MuteRoleID = "0";
                await Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{role.Name}** (server mute role) has been deleted.").Build());
                await GuildConfig.SaveAsync(Config, Guild.Id);
                return;
            }
            if (Config.Events.LogState == true && Config.Events.ChannelCreate == true && Config.Events.LogChannel != "0")
            {
                var embed = new EmbedBuilder();
                embed.WithTitle($"🛡 {role.Name} has been deleted").AddField(x =>
                {
                    x.Name = "Role Name";
                    x.Value = role.Name;
                }).AddField(x =>
                {
                    x.Name = "Role Permissions";
                    x.Value = string.Join("\n", role.Permissions);
                }).WithSuccesColor();
                await Guild.GetLogChannel().SendMessageAsync("", embed: embed.Build());
            }
        }
        public static string GetPermissions(SocketRole Role)
        {
            var Perms = Role.Permissions;
            var AddReactions = Perms.AddReactions == true ? "Can Add Reactions" : null;
            var Administrator = Perms.Administrator == true ? "Granted Administrator" : null;
            var X = Perms.AttachFiles == true ? "Can Attach Files" : null;
            var Z = Perms.BanMembers == true ? "Can Ban Perms" : null;
            var n = Perms.ChangeNickname == true ? "Can Change Their Own Nickname" : null;
            var a = Perms.Connect == true ? "Can Connect to Voicechannels" : null;
            var b = Perms.CreateInstantInvite == true ? "Can Create Instant Invite" : null;
            
            return AddReactions + "\n" + Administrator + "\n" + X + "\n" ;
        }
    }
}
