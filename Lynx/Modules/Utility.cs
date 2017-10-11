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
using Lynx.Interactive;
namespace Lynx.Modules
{
    public class Utility : LynxBase<LynxContext>
    {
        CommandService _service;
        GuildConfig GuildConfig = new GuildConfig();
        public Utility(CommandService service)
        {
            _service = service;
        }
        [Command("8ball", RunMode = RunMode.Async)]
        public async Task AskBallAsync([Remainder] string Question)
        {
            string[] PossibleAnswers = new string[] { "it is certain", "it is decidedly so", "without a doubt!", "yes definitely", "you may rely on it", "as I see it, yes"
            ,"most likely", "outlook good", "yes.", "signs point to yes", "no.", "nope", "reply hazy, try again", "ask again", "better not tell you now", "cannot predict now",
            "concentrate and ask again", "don't count on it", "my reply is no", "my sources say no", "outlook not so good", "very doubtful"};
            var RNG = new Random().Next(0, PossibleAnswers.Length);
            var FinalAnswer = PossibleAnswers[RNG];
            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("Asking the ball..").Build());
            await Task.Delay(1000);
            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(Context.User.Mention + ", " + FinalAnswer).Build());
        }
        [Command("inrole")]
        public async Task InroleAsync([Remainder] IRole role)
        {
            var Guild = Context.Guild as SocketGuild;
            IList<IUser> User_ = new List<IUser>();
            foreach(var User in Guild.Users.Where(x=>x.Roles.Contains(role)))
            {
                User_.Add(User);
            }
            var List = string.Join(", ", User_.OrderBy(x => new Random().Next()).Take(50));
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Inrole for **{role.Name}**```css\n{List}\n```").Build());
        }
        [Command("invite"), Alias("inv")]
        public async Task SendInviteAsync()
        {
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("[Invite me to your server!](https://discordapp.com/oauth2/authorize?client_id=357220874560077844&scope=bot&permissions=8)")
                .WithFooter(x=>
                {
                    x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl();
                    x.Text = Context.Client.CurrentUser.Username + " invite";
                }).Build());
        }
        [Command("botinfo"), Alias("stats", "binfo")]
        public async Task BotInfoAsync()
        {
            var Info = await (Context.Client as DiscordSocketClient).GetApplicationInfoAsync();
            var Config = Context.LynxConfig;
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithAuthor(x =>
            {
                x.Name = $"Hey im {Context.Client.CurrentUser.Username}";
                x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl();
            }).WithDescription("I'm a multifunctional discord bot for useful things. Please check out my [documentation](https://www.lynxbot.cf/documentation)!\n" +
            "[My GitHub repostory](https://github.com/dataccountzXJ9/Lynx)")
            
            .AddField(x =>
            {
                x.IsInline = true;
                x.Name = "Info";
                x.Value = $"<:developer:337976213584871434> **Developer:** `{Info.Owner}`\n<:www:337978348854575105> **Website:** [lynxbot.cf](https://www.lynxbot.cf)\n" +
            $"<:ramusage:338007753794650113> **Ram Usage:** {GetHeapSize()} MB\n" +
            $"<:uptime:338009832944566274> **Uptime:** {GetUptime()}\n" +
            $"<:envelop:338011365476401173> **Messages Received:** {Config.MessagesReceived}\n"+
            $"<:developer:337976213584871434> **Commands Fired:** {Config.CommandsTriggered}";
            }).WithSuccesColor().Build());
        }
        [Command("serverinfo"), Alias("ginfo", "sinfo")]
        public async Task ServerInfoAsync()
        {
            var Guild = Context.Guild as SocketGuild;
            int Msgs = 0;
            string Emotes = null;
            DateTime TimeStart = Process.GetCurrentProcess().StartTime.ToUniversalTime();
            TimeSpan uptime = DateTime.UtcNow - TimeStart;
            foreach (var TextChannel in Guild.TextChannels)
            {
                Msgs += TextChannel.CachedMessages.Count();
            }
            foreach(var Emoji in Guild.Emotes)
            {
                Emotes += $"<:{Emoji.Name}:{Emoji.Id}>";
            }
            var Description = $"**<:admin:338418960741695498> Owner:** {Guild.Owner}\n**<:users:337976192554762240> Users:** {Guild.Users.Count}\n**<:channels:337976246648569856> Channels:**\n    **Text Channels:** {Guild.TextChannels.Count}\n    **Voice Channels:** {Guild.VoiceChannels.Count}\n" +
            $"**<:roles:365518421968027649> Roles: ** {Context.Guild.Roles.Count}\n" +
            $"**<:roles:365518421968027649> Assignableroles: ** {Context.Config.Moderation.AssignableRoles.Count}\n" +
            $"**<:envelop:338011365476401173> Messages:** ({(Msgs / uptime.TotalMinutes).ToString("N2")}/minute)\n" +
            $"**<:info:365515689223651340> Emojis:** ({Guild.Emotes.Count})\n{Emotes}";
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(Description).WithFooter(x=> 
            {
                x.Text = Guild.Name;
                x.IconUrl = Guild.IconUrl;
            }
            ).WithThumbnailUrl(Guild.IconUrl).Build());
        }
        [Command("userinfo"), Alias("uinfo")]
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
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription(description).WithThumbnailUrl(user.GetAvatarUrl()).WithFooter(x => x.WithText($"Information about {user.Username}.").WithIconUrl(user.GetAvatarUrl())).Build());
        }
        [Command("help"), Alias("h")]
        public async Task GetCommandHelp([Remainder] string CommandName)
        {
            var embed = new EmbedBuilder();
            var ModuleInfo = _service.Search(Context, CommandName);
            if (!ModuleInfo.IsSuccess)
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("Command not found.").Build());
            foreach (var Command in ModuleInfo.Commands)
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
        [Command("modules"), Alias("moduletree")]
        public async Task GetModulesAsync()
        {
            var Modules = string.Join("\n", _service.Modules.GroupBy(m => m.GetTopLevelModule()).Select(m => "• " + m.Key.Name).OrderBy(s => s));
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithFooter(x => { x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl(); x.Text = DatabaseMethods.GetPrefix(Context.Guild as SocketGuild) +
                  "modulename to get a list of commands in that module.";
            }).AddField(x => { x.Name = "List of Modules:"; x.Value = Modules; }).WithSuccesColor().Build());
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
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithTitle(":notepad_spiral: Module Commands").WithFooter(x => { x.Text = Context.Guild.Name; x.IconUrl = Context.Guild.IconUrl; })
                .AddField(x => { x.IsInline = true; x.Name = "Commands"; x.Value = Commands; }).AddField(x => { x.IsInline = true; x.Value = Alias; x.Name = "Alias"; }).WithSuccesColor().Build());
        }
        public static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"hh\:mm\:ss");
        public static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
        
        

