using Discord.Commands;
using Lynx.Handler;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Lynx.Services.Embed;
using Lynx.Methods;
using Discord.WebSocket;
using Lynx.Services.Currency;
using Lynx.Database;

namespace Lynx.Modules
{
    public class Currency : LynxBase<LynxContext>
    {
        static GuildConfig GuildConfig = new GuildConfig();
        [Command("leaderboard")]
        public async Task LeadboardAsync(int Top = 10)
        {
            if (Top > 50)
                return;
            var Config = Context.Config;
            int Rank = 1;
            var Embed = new EmbedBuilder().WithSuccesColor();
            var toFetchFrom = Config.Currency.UsersList.OrderByDescending(x=>x.Value.TotalKarma).Take(Top);
            foreach (var User in toFetchFrom)
            {
                var User_ = await Context.Guild.GetUserAsync(Convert.ToUInt64(User.Key)) as IUser;
                Embed.AddField(x => { x.Name = @"\🏆" + $" {Rank++}. {User_.Username}"; x.Value = $"**Level:** {User.Value.Level} || **Total Karma:** {User.Value.TotalKarma}"; });
            }
            await Context.Channel.SendMessageAsync("", embed: Embed.WithTitle($"Leaderboard for {Context.Guild.Name}").WithDescription($"Currently showing Top {Rank-1}\n\n\n").WithThumbnailUrl(Context.Guild.IconUrl).WithFooter(x=> { x.Text = Context.Guild.Name + " leaderboard";
                x.IconUrl = Context.Guild.IconUrl;
            }).Build());
        }
        [Command("rank")]
        public async Task RankAsync(SocketGuildUser User = null)
        {
            if (User == null)
                User = Context.User as SocketGuildUser;
                await Context.Channel.SendProfileAsync(null, User);
        }
        [Command("daily")]
        public async Task DailyAsync()
        {
            var Config = Context.Config;
            var Profile = Config.Currency.UsersList[Context.User.Id.ToString()];
            int Difference = DateTime.Compare(Profile.LastCredit, DateTime.Now);
            if((Profile.LastCredit.ToString() == "0001-01-01 00:00:00") || (Profile.LastCredit.DayOfYear < DateTime.Now.DayOfYear && Difference < 0 || Difference >= 0))
            {
                    Profile.LastCredit = DateTime.Now;
                    Profile.Credits += 150;
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithFooter(x => {
                    x.Text = Context.User.Username; x.IconUrl = Context.User.GetAvatarUrl();
                }).WithDescription("I've rewarded you with **150** credits, come back tommorow for more!").Build());
            }
            else
            {
                TimeSpan diff = DateTime.Now - Profile.LastCredit;
                TimeSpan di = new TimeSpan(23 - diff.Hours, 59 - diff.Minutes, 59 - diff.Seconds);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Your daily credits refresh in {di.Hours} hours, {di.Minutes} minutes and {di.Seconds} seconds.").WithFooter(x=> {
                    x.Text = Context.User.Username; x.IconUrl = Context.User.GetAvatarUrl();
                }).Build());
            }
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
        }
    }
}
