using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Services.Embed;
using System.IO;
using System.Threading.Tasks;

namespace Lynx.Modules
{
    [RequireOwner]
    public class BotOwner : LynxBase<LynxContext>
    {
        static LynxConfig LynxConfig = new LynxConfig();
        [Command("agame")]
        public async Task AddPlayingGame([Remainder] string Gamename)
        {
            var Config = Context.LynxConfig;
            Config.BotGames.Add(Gamename);
            await LynxConfig.SaveAsync(Config);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{Gamename}** has been added to status games.").Build());
        }
        [Command("rgame")]
        public async Task RemovePlayingGame([Remainder] string Gamename)
        {
            var Config = Context.LynxConfig;
            Config.BotGames.Remove(Gamename);
            await LynxConfig.SaveAsync(Config);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{Gamename}** has been removed from status games.").Build());
        }
        [Command("Avatar")]
        public async Task AvatarAsync([Remainder] string Path)
        {
            using (var stream = new FileStream(Path, FileMode.Open))
            {
                await Context.Client.CurrentUser.ModifyAsync(x
                    => x.Avatar = new Image(stream));
                stream.Dispose();
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("Avatar has been updated.").Build());
        }
        [Command("username")]
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
        [Command("clarifaiapi")]
        public async Task SetClarifaiApi(string APIKey)
        {
            await Context.Message.DeleteAsync();
            var Config = Context.LynxConfig;
            Config.ClarifaiAPIKey = APIKey;
            await LynxConfig.SaveAsync(Config);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Clarifai API Key has been **updated** globally.").Build());
        }
    }
}

   