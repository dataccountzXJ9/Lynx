using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public class EnsureSourceUserCriterion : ICriterion<SocketMessage>
    {
        public Task<bool> JudgeAsync(LynxContext sourceContext, SocketMessage parameter)
        {
            var ok = sourceContext.User.Id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}