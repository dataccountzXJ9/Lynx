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
        public static string GetPrefix(this SocketGuild Guild)
        {
            string Prefix = null;
            if (Guild.LoadServerConfig().ServerPrefix == null)
            {
                Prefix = Guild.LoadBotConfig().BotPrefix;
            }
            else
            {
                Prefix = Guild.LoadServerConfig().ServerPrefix;
            }
            return Prefix;
        }
        public static string GetPrefix(this IGuild Guild)
        {
            string Prefix = null;
            if (Guild.LoadServerConfig().ServerPrefix == null)
            {
                Prefix = Guild.LoadBotConfig().BotPrefix;
            }
            else
            {
                Prefix = Guild.LoadServerConfig().ServerPrefix;
            }
            return Prefix;
        }
        public static ITextChannel GetJoinLogChannel(this SocketGuild Guild)
        {
            ITextChannel JoinChannel = null;
            if (Guild.LoadServerConfig().Events.Join.TextChannel != "0" && Guild.GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Join.TextChannel)) != null)
            {
                JoinChannel = Guild.GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Join.TextChannel)) as ITextChannel;
            }
            else if(Guild.LoadServerConfig().Events.Join.TextChannel == "0")
            {
                JoinChannel = Guild.DefaultChannel as ITextChannel;
            }
            return JoinChannel;
        }
        public static ITextChannel GetJoinLogChannel(this IGuild Guild)
        {
            ITextChannel JoinChannel = null;
            if (Guild.LoadServerConfig().Events.Join.TextChannel != "0" && (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Join.TextChannel)) != null)
            {
                JoinChannel = (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Join.TextChannel)) as ITextChannel;
            }
            else if (Guild.LoadServerConfig().Events.Join.TextChannel == "0")
            {
                JoinChannel = (Guild as SocketGuild).DefaultChannel as ITextChannel;
            }
            return JoinChannel;
        }
        public static ITextChannel GetLeaveLogChannel(this SocketGuild Guild)
        {
            ITextChannel LeaveChannel = null;
            if (Guild.LoadServerConfig().Events.Leave.TextChannel != "0" && Guild.GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Leave.TextChannel)) != null)
            {
                LeaveChannel = Guild.GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Leave.TextChannel)) as ITextChannel;
            }
            else if (Guild.LoadServerConfig().Events.Leave.TextChannel == "0")
            {
                LeaveChannel = Guild.DefaultChannel as ITextChannel;
            }
            return LeaveChannel;
        }
        public static ITextChannel GetLeaveLogChannel(this  IGuild Guild)
        {
            ITextChannel LeaveChannel = null;
            if (Guild.LoadServerConfig().Events.Leave.TextChannel != "0" && (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Leave.TextChannel)) != null)
            {
                LeaveChannel = (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.Leave.TextChannel)) as ITextChannel;
            }
            else if (Guild.LoadServerConfig().Events.Leave.TextChannel == "0")
            {
                LeaveChannel = (Guild as SocketGuild).DefaultChannel as ITextChannel;
            }
            return LeaveChannel;
        }
        public static ITextChannel GetLogChannel(this SocketGuild Guild)
        {
            ITextChannel LogChannel = null;
            var Config = Guild.LoadServerConfig();
            if(Config.Events.LogChannel == "")
            {
                return null;
            }
            else if(Config.Events.LogChannel != "0")
            {
                LogChannel = Guild.GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.LogChannel)) as ITextChannel;
            }
            return LogChannel;
        }
        public static ITextChannel GetLogChannel(this IGuild Guild)
        {
            ITextChannel LogChannel = null;
            var Config = Guild.LoadServerConfig();
            if (Config.Events.LogChannel == "")
            {
                return null;
            }
            else if (Config.Events.LogChannel != "0")
            {
                LogChannel = (Guild as SocketGuild).GetTextChannel(Convert.ToUInt64(Guild.LoadServerConfig().Events.LogChannel)) as ITextChannel;
            }
            return LogChannel;
        }
    }
}
