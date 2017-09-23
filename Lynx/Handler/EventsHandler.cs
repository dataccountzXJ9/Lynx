using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lynx.Database;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord;
using Lynx.Methods;
using Lynx.Services.Embed;
using Lynx.Modules;
using System.Linq;
using System.Threading;

namespace Lynx.Handler
{
    public class EventsHandler
    {
        IServiceProvider provider;
        CommandService commands;
        static DiscordSocketClient Client;
        public static bool Banned = false;
        public static bool Kicked = false;
        public static bool Unmuted = false;
        public static bool Muted = false;
        public static bool Deleted = false;
        public EventsHandler(IServiceProvider Prov)
        {
            provider = Prov;
            Client = provider.GetService<DiscordSocketClient>();
            commands = provider.GetService<CommandService>();
            Client.GuildAvailable += async (Guild) =>
            {
                await Guild.CheckGuildConfig();
            };
            Client.JoinedGuild += async (Guild) =>
            {
                await Guild.CheckGuildConfig();
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
            Client.Ready += async () =>
            {

                //    Services.Mute.Extensions.RemoveUser(Client);
                await Task.CompletedTask;
            };
        }
        static Timer Timer;
        internal static Task Load()
        {
            Timer = new Timer(_ =>
            {
                foreach (var Guild in Client.Guilds)
                {
                    var Config = Guild.LoadServerConfig();
                    var Mutelist = Config.Moderation.MuteList;
                    Task.WhenAll(Mutelist.Select(async snc =>
                    {
                        if (DateTime.Now > snc.Value.UnmuteTime)
                        {
                            Unmuted = true;
                            var User = Guild.GetUser(Convert.ToUInt64(snc.Key)) as SocketGuildUser;
                            await (User as SocketGuildUser).RemoveRoleAsync((Guild.GetRole(Convert.ToUInt64(Guild.LoadServerConfig().Moderation.MuteRoleID))));
                            await Guild.GetJoinLogChannel().SendMessageAsync($"", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from text and voice chat.").WithFooter(x =>
                            {
                                x.Text = $"{User} | [Automatic Message]";
                                x.IconUrl = User.GetAvatarUrl();
                            }).Build());
                            await Guild.GetLogChannel().SendMessageAsync($"", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from text and voice chat.").WithFooter(x =>
                            {
                                x.Text = $"{User} | [Automatic Message]";
                                x.IconUrl = User.GetAvatarUrl();
                            }).Build());
                            await Guild.UpdateMuteList(User as IUser, null, UpdateHandler.MuteOption.Unmute);
                        }
                    }));
                }
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }
        internal async Task OnUserJoin(SocketUser User)
        {
            var Guild = (User as SocketGuildUser).Guild;
            var Config = Guild.LoadServerConfig();
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
            if (Banned == false)
            {
                var Guild = (User as SocketGuildUser).Guild;
                var Config = Guild.LoadServerConfig();
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
            var Config = Guild.LoadServerConfig();
            if (Config.Events.LogState == true && Config.Events.UserBan == true && Config.Events.LogChannel != "0")
            {
                await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.BanLogEmbed(User).Build());
            }
        }
        internal async Task OnUserUnbanned(SocketUser User, SocketGuild Guild)
        {
            var Config = Guild.LoadServerConfig();
            if (Config.Events.LogState == true && Config.Events.UserUnban == true && Config.Events.LogChannel != "0")
            {
                await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.UnbanLogEmbed(User).Build());
            }
        }
        internal async Task OnGuildPresenceUpdated(SocketUser OutdatedUser, SocketUser UpdatedUser)
        {
            var UUser = (UpdatedUser as SocketGuildUser);
            var OUser = (OutdatedUser as SocketGuildUser);
            var Config = UUser.Guild.LoadServerConfig();
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
                    await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.Game).Build());
                }
                else if (!OUser.Roles.SequenceEqual(UUser.Roles))
                {
                    if (OUser.Roles.Count < UUser.Roles.Count)
                    {
                        if (Muted == true) return;
                        var diffRoles = UUser.Roles.Where(r => !OUser.Roles.Contains(r)).Select(r => r.Name);
                        await (Guild.GetLogChannel()).SendMessageAsync("", embed: EmbedMethods.PresenceLogEmbed(OUser, UUser, EmbedMethods.Presence.UserRoleAdded, diffRoles).Build());
                    }
                    else if (OUser.Roles.Count > UUser.Roles.Count)
                    {
                        if (Unmuted == true) return;
                        var diffRoles = OUser.Roles.Where(r => !UUser.Roles.Contains(r)).Select(r => r.Name);
                        var MuteList = Guild.LoadServerConfig().Moderation;
                        var User_ = MuteList.MuteList.TryGetValue(UUser.Id.ToString(), out MuteWrapper Value);
                        if (diffRoles.Contains("Lynx-Mute") && Value.UnmuteTime >= DateTime.Now)
                        {
                            await Guild.UpdateMuteList(UpdatedUser as IUser, null, UpdateHandler.MuteOption.Unmute);
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
            var Config = UUser.Guild.LoadServerConfig();
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
            var Config = Guild.LoadServerConfig();
            if (Config.Events.LogState == true && Config.Events.MessageUpdate == true && Config.Events.LogChannel != "0")
            {
                await (Channel as SocketTextChannel).Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.MessageUpdatedEmbed(Cache, Message).Build());
            }
        }
        internal async Task OnMessageDeleted(Cacheable<IMessage, ulong> Cache, ISocketMessageChannel Channel)
        {
            if (Deleted == true) return;
            if (Cache.Value.Author == Client.CurrentUser || Cache.Value.Author.IsBot == true) return;
            var Before = (Cache.HasValue ? Cache.Value : null) as IUserMessage;

            var Guild = (Cache.Value.Channel as SocketTextChannel).Guild;
            var Config = Guild.LoadServerConfig();
            if (Config.Events.LogState == true && Config.Events.MessageDelete == true && Config.Events.LogChannel != "0")
            {
                await (Channel as SocketTextChannel).Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.MessageDeletedEmbed(Cache, Before).Build());
            }
        }
        internal async Task OnChannelUpdated(SocketChannel OutdatedChannel, SocketChannel UpdatedChannel)
        {
            var Guild = (UpdatedChannel as SocketTextChannel).Guild;
            var Config = Guild.LoadServerConfig();
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
            var Config = Guild.LoadServerConfig();
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
            var Config = Guild.LoadServerConfig();
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
            var Config = Guild.LoadServerConfig();
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
            var Config = Guild.LoadServerConfig();
            if (role.Id.ToString() == Config.Moderation.MuteRoleID)
            {
                await Guild.UpdateServerModeration(UpdateHandler.Moderation.MuteRole);
                await Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{role.Name}** (server mute role) has been deleted.").Build());
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
