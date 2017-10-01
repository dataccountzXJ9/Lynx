using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Handler;

namespace Lynx.Interactive
{
    internal class EnsureIsIntegerCriterion : ICriterion<SocketMessage>
    {
        public Task<bool> JudgeAsync(LynxContext sourceContext, SocketMessage parameter)
        {
            bool ok = int.TryParse(parameter.Content, out _);
            return Task.FromResult(ok);
        }
    }
}