using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Lynx.Interactive
{
    public class EnsureFromUserCriterion : ICriterion<SocketMessage>
    {
        private readonly ulong _id;
        public EnsureFromUserCriterion(ulong id)
            => _id = id;

        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketMessage parameter)
        {
            bool ok = _id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}