using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Handler;

namespace Lynx.Interactive
{
    internal class EnsureReactionFromSourceUserCriterion : ICriterion<SocketReaction>
    {
        public Task<bool> JudgeAsync(LynxContext sourceContext, SocketReaction parameter)
        {
            bool ok = parameter.UserId == sourceContext.User.Id;
            return Task.FromResult(ok);
        }
    }
}