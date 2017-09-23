using Discord;
using Discord.WebSocket;
using Lynx.Handler;
using Lynx.Methods;
using Lynx.Services.Embed;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace Lynx.Services.Mute
{
    public static class Extensions
    {
        public static DateTime GetTime(int Value, string Time)
        {
            var DT = DateTime.Now;
            switch (Time.ToLowerInvariant())
            {
                case "hour":
                case "hours":
                    return DT.AddHours(Value);
                case "minutes":
                case "minute":
                    return DT.AddMinutes(Value);
                case "day":
                case "days":
                    return DT.AddDays(Value);
                case "second":
                case "seconds":
                    return DT.AddSeconds(Value);
                default:
                    return DT;
            }
        }
    }
}
