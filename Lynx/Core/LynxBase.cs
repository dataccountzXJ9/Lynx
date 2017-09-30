using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Lynx.Database;
using Lynx.Handler;

namespace Lynx
{
    public class LynxBase<T> : ModuleBase<LynxContext> where T: LynxContext
    {
        public static IServiceProvider Provider { get; set; }
        protected override async Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            _ = Provider.GetRequiredService<GuildConfig>().SaveAsync(Context.Config, Context.Guild.Id);
            _ = Provider.GetRequiredService<LynxConfig>().SaveAsync(Context.LynxConfig);
            return await Context.Channel.SendMessageAsync(message, isTTS, embed, options);
        }
    }
}
