using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public class EnsureSourceChannelCriterion : ICriterion<SocketMessage>
    {
        public Task<bool> JudgeAsync(LynxContext sourceContext, SocketMessage parameter)
        {
            var ok = sourceContext.Channel.Id == parameter.Channel.Id;
            return Task.FromResult(ok);
        }
    }
}