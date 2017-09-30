using Discord.WebSocket;
using Lynx.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Handler
{
    public class CustomReactionHandler
    {
        static GuildConfig GuildConfig = new GuildConfig();
        public static async Task CustomReactionService(SocketMessage Message)
        {
            CustomReactionWrapper Reaction = GetReaction(Message);
            await Message.Channel.SendMessageAsync(Reaction.Response);
        }
        public static CustomReactionWrapper GetReaction(SocketMessage Message)
        {
            var Message_ = Message.Content?.ToLowerInvariant();
            var Guild = (Message.Channel as SocketTextChannel).Guild;
            var Config = GuildConfig.LoadAsync(Guild.Id);
            var CR_ = Config.CustomReactions.Where(cr =>
            {
                if (cr == null)
                    return false;
                return (cr.Trigger == Message_);
            }).ToArray();
            CustomReactionWrapper toReturn = null;
            if (CR_.Length != 0)
            {
                var Reaction = CR_[new Random().Next(0, CR_.Length)];
                if(Reaction != null)
                {
                    toReturn = Reaction;
                }
            }
            return toReturn;
        }
    }
}
