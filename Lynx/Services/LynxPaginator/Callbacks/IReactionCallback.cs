using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public interface IReactionCallback
    {
        RunMode RunMode { get; }
        ICriterion<SocketReaction> Criterion { get; }
        TimeSpan? Timeout { get; }
        LynxContext Context { get; }

        Task<bool> HandleCallbackAsync(SocketReaction reaction);
    }
}