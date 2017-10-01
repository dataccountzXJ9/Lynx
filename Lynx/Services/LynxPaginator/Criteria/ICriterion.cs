using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public interface ICriterion<T>
    {
        Task<bool> JudgeAsync(LynxContext sourceContext, T parameter);
    }
}