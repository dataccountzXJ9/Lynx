using System.Threading.Tasks;
using Discord.Commands;
namespace Lynx.Interactive
{
    public class EmptyCriterion<T> : ICriterion<T>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter)
            => Task.FromResult(true);
    }
}