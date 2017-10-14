using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Models.Database;
using Lynx.Services.Embed;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static NSFWModels;

namespace Lynx.Modules
{
    [RequireOwner]
    public class BotOwner : LynxBase<LynxContext>
    {
        static LynxConfig LynxConfig = new LynxConfig();
        static GuildConfig GuildConfig = new GuildConfig();
        MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.Unicode.GetBytes(value ?? ""));
        }
        IEnumerable<Assembly> Assemblies => GetAssemblies();
        IEnumerable<string> Imports => Context.LynxConfig.EvalImports;
        [Command("agbg")]
        public async Task AGBG(string BG)
        {
            var Client = Context.Client as DiscordSocketClient;
            foreach(var Guild in Client.Guilds)
            {
                var Config = GuildConfig.LoadAsync(Guild.Id);
                foreach(var user in Config.Currency.UsersList)
                {
                    user.Value.Backgrounds.NotOwned.Add(BG);
                }
                await GuildConfig.SaveAsync(Config, Guild.Id);
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($"Succesfully added Background `{BG}` to all users across {Client.Guilds.Count} servers.").Build());
        }
        [Command("rgbg")]
        public async Task RGBG(string BG)
        {
            
            var Client = Context.Client as DiscordSocketClient;
            foreach (var Guild in Client.Guilds)
            {
                var Config = GuildConfig.LoadAsync(Guild.Id);
                foreach (var user in Config.Currency.UsersList)
                {
                    user.Value.Backgrounds.NotOwned.Remove(BG);
                }
                await GuildConfig.SaveAsync(Config, Guild.Id);
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription($"Succesfully added Background `{BG}` to all users across {Client.Guilds.Count} servers.").Build());
        }
        [Command("addgame"), Alias("agame")]
        public async Task AddPlayingGame([Remainder] string Gamename)
        {
            var Config = Context.LynxConfig;
            Config.BotGames.Add(Gamename);
            await LynxConfig.SaveAsync(Config);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{Gamename}** has been added to status games.").Build());
        }
        [Command("removegame"), Alias("rgame")]
        public async Task RemovePlayingGame([Remainder] string Gamename)
        {
            var Config = Context.LynxConfig;
            Config.BotGames.Remove(Gamename);
            await LynxConfig.SaveAsync(Config);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{Gamename}** has been removed from status games.").Build());
        }
        [Command("avatar"), Alias("avi", "botavatar")]
        public async Task AvatarAsync([Remainder] string Path)
        {
            using (var stream = new FileStream(Path, FileMode.Open))
            {
                await Context.Client.CurrentUser.ModifyAsync(x
                    => x.Avatar = new Discord.Image(stream));
                stream.Dispose();
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("Avatar has been updated.").Build());
        }
        [Command("username"), Alias("botusername")]
        public async Task UserNameAsync([Remainder] string UserName)
        {
            await Context.Client.CurrentUser.ModifyAsync(x => x.Username = UserName);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Changed bot username to: **{Context.Client.CurrentUser.Username}**").Build());
        }
        [Command("debug")]
        public async Task ToggleDebug()
        {
            var Config = Context.LynxConfig;
            var State = Config.Debug == true ? "off" : "on";
            Config.Debug = !Config.Debug;
            await LynxConfig.SaveAsync(Config);
            await ReplyAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**Debug Mode** has been succesfully turned **{State}**.").Build());
        }
        [Command("clarifaiapi"), Alias("capi")]
        public async Task SetClarifaiApi(string APIKey)
        {
            await Context.Message.DeleteAsync();
            var Config = Context.LynxConfig;
            Config.ClarifaiAPIKey = APIKey;
            await LynxConfig.SaveAsync(Config);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Clarifai API Key has been **updated** globally.").Build());
        }
        [Command("Eval"), Summary("Evaluates some sort of expression for you.")]
        public async Task EvalAsync([Remainder] string Code)
        {
            var Options = ScriptOptions.Default.AddReferences(Assemblies).AddImports(Imports);
            var Globals = new Globals
            {
                Client = Context.Client as DiscordSocketClient,
                Context = Context,
                Guild = Context.Guild as SocketGuild,
                Channel = Context.Channel as SocketGuildChannel,
                User = Context.User as SocketGuildUser,
                ServerConfig = Context.Config
            };
            try
            {
                var eval = await CSharpScript.EvaluateAsync(Code, Options, Globals, typeof(Globals));
                var embed = new EmbedBuilder().WithSuccesColor().WithFooter(x => { x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl(); x.Text = "Evaluated succesfully."; });
                embed.AddField(x =>
                {
                    x.Name = "Input";
                    x.Value = $"```{Code}```";
                })
                .AddField(x =>
                {
                    x.Name = "Output";
                    x.Value = $"```{eval.ToString() ?? "No Result."}```";
                });
                await ReplyAsync("", embed: embed.Build());
            }
            catch (Exception e)
            {
                var embed = new EmbedBuilder().WithSuccesColor().WithFooter(x => { x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl(); x.Text = "Failed to evaluate."; });
                embed.AddField(x =>
                {
                    x.Name = "Input";
                    x.Value = $"```{Code}```";
                })
                .AddField(x =>
                {
                    x.Name = "Output";
                    x.Value = $"```{e.GetType().ToString()} : {e.Message}```";
                });
                await ReplyAsync("", embed: embed.Build());
            }
        }
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var Assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (var a in Assemblies)
            {
                var asm = Assembly.Load(a);
                yield return asm;
            }
            yield return Assembly.GetEntryAssembly();
            yield return typeof(ILookup<string, string>).GetTypeInfo().Assembly;
        }
        public class Globals
        {
            public LynxContext Context { get; internal set; }
            public DiscordSocketClient Client { get; internal set; }
            public SocketGuildUser User { get; internal set; }
            public SocketGuild Guild { get; internal set; }
            public SocketGuildChannel Channel { get; internal set; }
            public ServerModel ServerConfig { get; internal set; }
        }
    }
}

   