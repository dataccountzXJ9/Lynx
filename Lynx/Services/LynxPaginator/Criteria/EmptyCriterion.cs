using System.Threading.Tasks;
using Discord.Commands;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public class EmptyCriterion<T> : ICriterion<T>
    {
        public Task<bool> JudgeAsync(LynxContext sourceContext, T parameter)
            => Task.FromResult(true);
    }
}