using Discord;
using Discord.Commands;
using Lynx.Handler;
using Lynx.Services.Embed;
using System.Threading.Tasks;

namespace Lynx.Modules
{
    [RequireOwner]
    public class BotOwner : ModuleBase
    {
        [Command("debug")]
        public async Task ToggleDebug()
        {
            var Config = Context.Guild.LoadBotConfig();
            var State = Config.Debug == true ? "off" : "on";
            await Context.Client.UpdateBotConfig(UpdateHandler.BotConfig.Debug);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**Debug Mode** has been succesfully turned **{State}**.").Build());
        }
        [Command("clarifaiapi")]
        public async Task SetClarifaiApi(string APIKey)
        {
            await Context.Message.DeleteAsync();
            var Config = Context.Guild.LoadBotConfig();
            await Context.Client.UpdateBotConfig(UpdateHandler.BotConfig.ClarifaiAPIKey, APIKey);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Clarifai API Key has been **updated** globally.").Build());
        }
    }
}

   