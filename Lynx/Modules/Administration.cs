using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Handler;
using Lynx.Services.Embed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lynx.Modules
{
    public class Administration : ModuleBase
    {
        [Command("iam")]
        public async Task IAmAsync(IRole Role)
        {
            EventsHandler.Muted = true;
            var Config = Context.Guild.LoadServerConfig().Moderation;
            if (Config.AssignableRoles.Contains(Role.Id.ToString()))
            {
                try
                {
                    await (Context.User as SocketGuildUser).AddRoleAsync(Role);
                }
                catch
                {
                    await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"I can not assign **{Role.Name}** because it is higher than my role.").Build());
                    return;
                }
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You now have **{Role.Name}**.").Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Role.Name}** is not self-assignable.").Build());
                return;
            }
        }
        [Command("iamn")]
        public async Task IAmnAsync(IRole Role)
        {
            EventsHandler.Unmuted = true;
            var Config = Context.Guild.LoadServerConfig().Moderation;
            if (Config.AssignableRoles.Contains(Role.Id.ToString()))
            {
                await (Context.User as SocketGuildUser).RemoveRoleAsync(Role);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You no longer have **{Role.Name}**.").Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Role.Name}** is not self-assignable.").Build());
                return;
            }
        }
        [Command("lsar")]
        public async Task ListSelfAssignAbleRolesAsync()
        {
            int i = 0;
            var Config = Context.Guild.LoadServerConfig().Moderation.AssignableRoles;
            if (Config.Count == 0)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"**{Context.Guild.Name}'s** selfassignablelist is **empty**.").Build());
                return;
            }
            IList<IRole> RoleList = new List<IRole>();
            foreach (var Role in Config)
            {
                var Role_ = Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole;
                if (Role_ == null)
                {
                    await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.RemoveSelfassignableRole, Convert.ToUInt64(Role));
                    return;
                }
                else
                {
                    RoleList.Add(Context.Guild.GetRole(Convert.ToUInt64(Role)) as IRole);
                }
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Assignable roles for **{Context.Guild.Name}**\n" + $@"```css
{string.Join("\n", RoleList.GroupBy(x => (i++) / 3).Select(L9 => string.Concat(L9.Select(F => $"{F.Name,-15}"))))}
```").Build());
        }
    }
}
