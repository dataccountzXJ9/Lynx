using Discord;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Methods
{
    public static class DatabaseMethods
    {
        static GuildConfig GuildConfig = new GuildConfig();
        static LynxConfig LynxConfig = new LynxConfig();
        public static string GetPrefix(this SocketGuild Guild)
        {
            var Config = GuildConfig.LoadAsync(Guild.Id);
            string Prefix = null;
            if (Config.ServerPrefix == null)
            {
                Prefix = LynxConfig.LoadConfig.BotPrefix;
            }
            else
            {
                Prefix = Config.ServerPrefix;
            }
            return Prefix;
        }
        public static string GetPrefix(this IGuild Guild)
        {
            var Config = GuildConfig.LoadAsync(Guild.Id);
            string Prefix = null;
            if (Config.ServerPrefix == null)
            {
                Prefix = LynxConfig.LoadConfig.BotPrefix;
            }
            else
            {
                Prefix = Config.ServerPrefix;
            }
            return Prefix;
        }
        public static ITextChannel GetJoinLogChannel(this SocketGuild Guild)
        {
            ITextChannel JoinChannel = null;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.Join.TextChannel != "0" && Guild.GetTextChannel(Convert.ToUInt64(Config.Events.Join.TextChannel)) != null)
            {
                JoinChannel = Guild.GetTextChannel(Convert.ToUInt64(Config.Events.Join.TextChannel)) as ITextChannel;
            }
            else if(Config.Events.Join.TextChannel == "0")
            {
                JoinChannel = Guild.DefaultChannel as ITextChannel;
            }
            return JoinChannel;
        }
        public static ITextChannel GetJoinLogChannel(this IGuild Guild)
        {
            ITextChannel JoinChannel = null;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            var _ = Guild as SocketGuild;
            if (Config.Events.Join.TextChannel != "0" && _.GetTextChannel(Convert.ToUInt64(Config.Events.Join.TextChannel)) != null)
            {
                JoinChannel = _.GetTextChannel(Convert.ToUInt64(Config.Events.Join.TextChannel)) as ITextChannel;
            }
            else if (Config.Events.Join.TextChannel == "0")
            {
                JoinChannel = _.DefaultChannel as ITextChannel;
            }
            return JoinChannel;
        }
        public static ITextChannel GetLeaveLogChannel(this SocketGuild Guild)
        {
            ITextChannel LeaveChannel = null;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.Leave.TextChannel != "0" && Guild.GetTextChannel(Convert.ToUInt64(Config.Events.Leave.TextChannel)) != null)
            {
                LeaveChannel = Guild.GetTextChannel(Convert.ToUInt64(Config.Events.Leave.TextChannel)) as ITextChannel;
            }
            else if (Config.Events.Leave.TextChannel == "0")
            {
                LeaveChannel = Guild.DefaultChannel as ITextChannel;
            }
            return LeaveChannel;
        }
        public static ITextChannel GetLeaveLogChannel(this  IGuild Guild)
        {
            ITextChannel LeaveChannel = null;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.Leave.TextChannel != "0" && (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Config.Events.Leave.TextChannel)) != null)
            {
                LeaveChannel = (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Config.Events.Leave.TextChannel)) as ITextChannel;
            }
            else if (Config.Events.Leave.TextChannel == "0")
            {
                LeaveChannel = (Guild as SocketGuild).DefaultChannel as ITextChannel;
            }
            return LeaveChannel;
        }
        public static ITextChannel GetLogChannel(this SocketGuild Guild)
        {
            ITextChannel LogChannel = null;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogChannel == "")
            {
                return null;
            }
            else if(Config.Events.LogChannel != "0")
            {
                LogChannel = Guild.GetTextChannel(Convert.ToUInt64(Config.Events.LogChannel)) as ITextChannel;
            }
            return LogChannel;
        }
        public static ITextChannel GetLogChannel(this IGuild Guild)
        {
            ITextChannel LogChannel = null;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            if (Config.Events.LogChannel == "")
            {
                return null;
            }
            else if (Config.Events.LogChannel != "0")
            {
                LogChannel = (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Config.Events.LogChannel)) as ITextChannel;
            }
            return LogChannel;
        }
    }
}
