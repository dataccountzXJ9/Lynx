using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lynx.Methods;
using Lynx.Services.Embed;
using System.Linq;
using Lynx.Services.Help;
using static Lynx.Services.Help.HelpExtension;
using Discord.WebSocket;
using Lynx.Database;
using System.Diagnostics;
using Lynx.Handler;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Linq;
namespace Lynx.Modules
{
    public class Utility : ModuleBase<LynxContext>
    {
        CommandService _service;
        public Utility(CommandService service)
        {
            _service = service;
        }
        [Command("serverinfo")]
        public async Task ServerInfoAsync()
        {
            var Guild = Context.Guild as SocketGuild;
            int Msgs = 0;
            DateTime TimeStart = Process.GetCurrentProcess().StartTime.ToUniversalTime();
            TimeSpan uptime = DateTime.UtcNow - TimeStart;
            foreach (var TextChannel in Guild.TextChannels)
            {
                Msgs = Msgs + TextChannel.CachedMessages.Count();
            }
            var Description = $"**<:admin:338418960741695498> Owner:** {Guild.Owner}\n**<:users:337976192554762240> Users:** {Guild.Users.Count}\n**<:help:338419741834346498> Default Channel:** {Guild.DefaultChannel.Mention}\n\n" +
            $"**<:Discord:337975837464854530> Roles:** {Context.Guild.Roles.Count}\n" +
            $"**<:notepad:338401542900023298> Assignableroles:** {Context.Config.Moderation.AssignableRoles.Count}\n" +
            $"**<:envelop:338011365476401173> Messages:** ({(Msgs / uptime.TotalMinutes).ToString("N2")}/minute)\n";
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(Description).Build());
        }
        [Command("userinfo")]
        public async Task UserInfoAsync(SocketGuildUser user = null)
        {
            if (user == null)
                user = Context.User as SocketGuildUser;
            var game = user.Game.HasValue ? $"{user.Game.Value.Name}" : "N/A";
            string description = $"**<:Discord:337975837464854530> Username:** {user.Username}#{user.Discriminator}\n" +
            $"**<:Discord:337975837464854530> Status:** {user.Status}\n" +
            $"**<:game:338422429686824960> Game:** {game}\n" +
            $"**<:Discord:337975837464854530> Roles:** {user.Roles.Count}\n" +
            $"**<:uptime:338009832944566274> Created:** {user.CreatedAt.ToString(@"yyyy-MM-dd")}";
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(description).WithThumbnailUrl(user.GetAvatarUrl()).WithFooter(x=>x.WithText($"Information about {user.Username}.").WithIconUrl(user.GetAvatarUrl())).Build());
        }
        [Command("help")]
        public async Task GetCommandHelp([Remainder] string CommandName)
        {
            var embed = new EmbedBuilder();
            var ModuleInfo = _service.Search(Context, CommandName);
            if (!ModuleInfo.IsSuccess)
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("Command not found.").Build());
            foreach(var Command in ModuleInfo.Commands)
            {
                var Command_ = Command.Command;
                string Alias = null;
                if (string.IsNullOrWhiteSpace(string.Join(", ", Command_.Aliases)))
                    Alias = "Command has no Aliases.";
                else
                    Alias = string.Join(", ", Command_.Aliases);

                string Parameters = null;
                if (string.IsNullOrWhiteSpace(string.Join(", ", Command_.Parameters.Select(p => p.Name))))
                    Parameters = "Command requires no parameters.";
                else
                    Parameters = string.Join(", ", Command_.Parameters.Select(p => p.Name));
                embed.WithAuthor(x => { x.Name = "Command Info"; x.IconUrl = Context.Guild.IconUrl; });
                embed.Description = $"**Aliases:** {Alias}\n**Parameters:** {Parameters}\n**Summary:** {Command_.Summary}";
            }
            await Context.Channel.SendMessageAsync("", embed: embed.WithSuccesColor().Build());
        }
        [Command("modules")]
        public async Task GetModulesAsync()
        {
            var Modules = string.Join("\n", _service.Modules.GroupBy(m => m.GetTopLevelModule()).Select(m => "• " + m.Key.Name).OrderBy(s => s));
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithFooter(x=>{ x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl(); x.Text = DatabaseMethods.GetPrefix(Context.Guild as SocketGuild) +
                "modulename to get a list of commands in that module.";
            }).AddField(x=> { x.Name = "List of Modules:"; x.Value = Modules; }).WithSuccesColor().Build());
        }
        [Command("commands"), Alias("cmds")]
        public async Task GetCommandsByModuleNameAsync([Remainder] string module)
        {
            module = module?.Trim().ToUpperInvariant();
            var Prefix = (Context.Guild as SocketGuild).GetPrefix();
            var Module = _service.Commands.Where(x => x.Module.GetTopLevelModule().Name.ToUpperInvariant().StartsWith(module))
                .OrderBy(c => c.Aliases.First())
                .Distinct(new CommandTextEqualityComparer())
                .AsEnumerable();
            if (!Module.Any())
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription("Module not found.").WithFailedColor().Build());
                return;
            }
            string Commands = null;
            string Alias = null;
            foreach (var Command in Module)
            {
                Commands = Commands + Prefix + string.Join(" ", Command.Aliases.FirstOrDefault()) + "\n";
                Alias = Alias + "[" + string.Join(", ", Command.Aliases.Skip(1)) + "]\n" ?? "[" + Prefix + string.Join(", ", Command.Aliases.Skip(1)) + "]\n";
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithTitle(":notepad_spiral: Module Commands").WithFooter(x=> { x.Text = Context.Guild.Name;x.IconUrl = Context.Guild.IconUrl; })
                .AddField(x=> { x.IsInline = true;x.Name = "Commands"; x.Value = Commands; }).AddField(x=> { x.IsInline = true;x.Value = Alias;x.Name = "Alias"; }).WithSuccesColor().Build());
        }
    }
}
