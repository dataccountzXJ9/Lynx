using Discord;
using Discord.Commands;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Interactive;
using Lynx.Services.Embed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lynx.Modules
{
    public class CustomReactions : LynxBase<LynxContext>
    {
        static GuildConfig GuildConfig = new GuildConfig();
        private static readonly PaginatedAppearanceOptions AOptions = new PaginatedAppearanceOptions()
        {
            Timeout = TimeSpan.FromMinutes(60)
        };
        [Command("acr")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        public async Task AddCustomReactionAsync(string Name, [Remainder] string Response)
        {
            var Config = Context.Config;
            int ID = 0;
            try
            {
                ID = Config.CustomReactions.Select(x => x.Id).Max();
            }
            catch { ID = 0; }
            ID++;
            Config.CustomReactions.Add(new CustomReactionWrapper { Id = ID, Response = Response, Trigger = Name });
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**I've created a custom reaction.**\n\n**ID:** `{ID}`\n**Trigger:** `{Name}`\n**Response:** `{Response}`").Build());
        }
        [Command("dcr")]
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        public async Task DeleteCustomReactionAsync(int Id)
        {
            var Config = Context.Config;
            if (Config.CustomReactions.Where(x => x.Id == Id).Any())
            {
                Config.CustomReactions.RemoveAll(x => x.Id == Id);
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"`#{Id}` has been **deleted**.").Build());
                return;
            }
            else
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"`#{Id}` **does not** exist").Build());
        }

        
        [Command("lcr")]
        public async Task ListCustomReactions()
        {
            var Config = Context.Config;
            string Page1 = null;
            string Page2 = null;
            string Page3 = null;
            string Page4 = null;
            string Page5 = null;
            string Page6 = null;
            string Page7 = null;
            string Page8 = null;
            string Page9 = null;
            string Page10 = null;
            string Page11 = null;
            string Page12 = null;
            string Page13 = null;
            string Page14 = null;
            string Page15 = null;
            foreach (var Page in Config.CustomReactions.Take(15))
            {
                if (Page == null)
                    return;
                Page1 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(15).Take(15))
            {
                if (Page == null)
                    return;
                Page2 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(30).Take(15))
            {
                if (Page == null)
                    return;
                Page3 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(45).Take(15))
            {
                if (Page == null)
                    return;
                Page4 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(60).Take(15))
            {
                if (Page == null)
                    return;
                Page5 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(75).Take(15))
            {
                if (Page == null)
                    return;
                Page6 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(90).Take(15))
            {
                if (Page == null)
                    return;
                Page7 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(105).Take(15))
            {
                if (Page == null)
                    return;
                Page8 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(120).Take(15))
            {
                if (Page == null)
                    return;
                Page9 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(135).Take(15))
            {
                if (Page == null)
                    return;
                Page10 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(150).Take(15))
            {
                if (Page == null)
                    return;
                Page11 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(165).Take(15))
            {
                if (Page == null)
                    return;
                Page12 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(180).Take(15))
            {
                if (Page == null)
                    return;
                Page13 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(195).Take(15))
            {
                if (Page == null)
                    return;
                Page14 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            foreach (var Page in Config.CustomReactions.Skip(210).Take(15))
            {
                if (Page == null)
                    return;
                Page15 += $"`#{Page.Id}` - {Page.Trigger}\n";
                continue;
            }
            List<string> Pages = new List<string>();
            if (Page1 != null)
                Pages.Add(Page1);
            if (Page2 != null)
                Pages.Add(Page2);
            if (Page3 != null)
                Pages.Add(Page3);
            if (Page4 != null)
                Pages.Add(Page4);
            if (Page5 != null)
                Pages.Add(Page5);
            if (Page6 != null)
                Pages.Add(Page6);
            if (Page7 != null)
                Pages.Add(Page7);
            if (Page8 != null)
                Pages.Add(Page8);
            if (Page9 != null)
            Pages.Add(Page9);
            if (Page10 != null)
                Pages.Add(Page10);
            if (Page11 != null)
                Pages.Add(Page11);
            if (Page12 != null)
                Pages.Add(Page12);
            if (Page13 != null)
                Pages.Add(Page13);
            if (Page14 != null)
                Pages.Add(Page14);
            if (Page15 != null)
                Pages.Add(Page15);
            var PaginatedMessage = new PaginatedMessage()
            {
                Author = new Discord.EmbedAuthorBuilder()
                {
                    IconUrl = Context.Guild.IconUrl,
                    Name = Context.Guild.Name + " Custom Reactions",
                },
                Options = AOptions,
                Pages = Pages,
                Color = Discord.Color.Green,
            };
            await PagedReplyAsync(PaginatedMessage);
        }
    }
}

