using Discord.Addons.Interactive;
using Discord.Commands;
using Lynx.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Modules
{
    public class CustomReactions : ModuleBase
    {
        static GuildConfig GuildConfig = new GuildConfig();
        private static readonly PaginatedAppearanceOptions AOptions = new PaginatedAppearanceOptions()
        {

            JumpDisplayOptions = JumpDisplayOptions.Never,
            DisplayInformationIcon = false,
            Timeout = TimeSpan.FromMinutes(60)

        };
        [Command("acr")]
        public async Task AddCustomReactionAsync(string Name, [Remainder] string Response)
        {
            var Config = GuildConfig.LoadAsync(Context.Guild.Id);
            int ID = 0;
            try
            {
                ID = Config.CustomReactions.Select(x => x.Id).Max();
            }
            catch { ID = 0; }
            ID++;
            Config.CustomReactions.Add(new CustomReactionWrapper { Id = ID, Response = Response, Trigger = Name });
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
        }
        [Command("dcr")]
        public async Task DeleteCustomReactionAsync(int Id)
        {
            var Config = GuildConfig.LoadAsync(Context.Guild.Id);
            Config.CustomReactions.RemoveAll(x => x.Id == Id);
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);

        }
        [Command("next")]
        public async Task Test_NextMessageAsync()
        {
            await ReplyAsync("What is 2+2?");
          //  var response = await NextMessageAsync();
         //   if (response != null)
        //        await ReplyAsync($"You replied: {response.Content}");
      //      else
       //         await ReplyAsync("You did not reply before the timeout");
        }
        [Command("lcr")]
        public async Task ListCustomReactions()
        {
            var paginatedMessage = new PaginatedMessage()
            {

                Pages = new[] { "xdTEST", "xdTEST2", "xdTEST3" },
                Options = AOptions,

            };
        //    await PagedReplyAsync(paginatedMessage);
        }
    }
}

