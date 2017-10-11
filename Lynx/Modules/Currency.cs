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
using System.Collections.Generic;
namespace Lynx.Modules
{
    public class Currency : LynxBase<LynxContext>
    {
        static GuildConfig GuildConfig = new GuildConfig();
        [Command("cup", RunMode = RunMode.Async)]
        public async Task CupGame(int bet = 25)
        {

            string[] ImagePaths = new[] { "https://cdn.discordapp.com/attachments/337325360087695368/367397031880425482/unknown.png", "https://cdn.discordapp.com/attachments/337325360087695368/367396700782067713/unknown.png",
                "https://cdn.discordapp.com/attachments/337325360087695368/367396204432195607/unknown.png", "https://cdn.discordapp.com/attachments/337325360087695368/367395077125046274/unknown.png", "https://cdn.discordapp.com/attachments/337325360087695368/367395557406146582/unknown.png" };
            var RNGPath = new Random().Next(1, ImagePaths.Length);
            var FinalCup = ImagePaths[RNGPath];
            var PlusOne = RNGPath + 1;
            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"{Context.User.Mention}, guess under which cup the golden coin is. [NO REWARDS ATM]").WithImageUrl("https://cdn.discordapp.com/attachments/337325360087695368/367400517107974145/unknown.png").Build());
            var Message = await NextMessageAsync();
            if (Message != null)
            {
                switch (Message.Content)
                {
                    case "1":
                        if(Convert.ToInt16(Message.Content) ==PlusOne)
                        {
                            await toModify.ModifyAsync(x=>x.Embed = new EmbedBuilder().WithSuccesColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you guessed right!").Build());
                        }
                        else
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithFailedColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you did not guess right.").Build());
                        }
                        break;
                    case "2":
                        if (Convert.ToInt16(Message.Content) == PlusOne)
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you guessed right!").Build());
                        }
                        else
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithFailedColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you did not guess right.").Build());
                        }
                        break;
                    case "3":
                        if (Convert.ToInt16(Message.Content) == PlusOne)
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you guessed right!").Build());
                        }
                        else
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithFailedColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you did not guess right.").Build());
                        }
                        break;
                    case "4":
                        if (Convert.ToInt16(Message.Content) == PlusOne)
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you guessed right!").Build());
                        }
                        else
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithFailedColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you did not guess right.").Build());
                        }
                        break;
                    case "5":
                        if (Convert.ToInt16(Message.Content) == PlusOne)
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you guessed right!").Build());
                        }
                        else
                        {
                            await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithFailedColor().WithImageUrl(FinalCup).WithDescription($"{Context.User.Mention}, you did not guess right.").Build());
                        }
                        break;
                }
            }

        }
        [Command("shop", RunMode = RunMode.Async)]
        public async Task ShopAsync()
        {
            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Type **1** for profile backgrounds\nType **2** for levelup backgrounds").Build());
            var Type = await NextMessageAsync(timeout: TimeSpan.FromSeconds(30));
            if (Type != null)
            {
                switch (Type.Content)
                {
                    case "1":
                        await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription("Type the name of the profile background you want to purchase.").Build());
                        
                        break;
                    case "2":
                        await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription("Type the name of the levelup background you want to purchase.").Build());
                        break;
                }
            }
        }
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [Command("blacklist"),Alias("bl", "blist")]
        public async Task BlackListChannelAsync(ITextChannel Channel = null)
        {
            if (Channel == null)
                Channel = Context.Channel as ITextChannel;
            var Config = Context.Config;
            Config.Currency.BlackListedChannels.Add(Channel.Id.ToString());
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"I've blacklisted {Channel.Mention}, users can no longer gain XP in there.").WithFooter(x =>
            {
                x.IconUrl = Context.Guild.IconUrl;
                x.Text = "Blacklisted " + Channel.Name;
            }).Build());
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
        }
        [RequireUserPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
        [Command("whitelist"), Alias("wl", "wlist")]
        public async Task WhiteListAsync(ITextChannel Channel = null)
        {
            if (Channel == null)
                Channel = Context.Channel as ITextChannel;
            var Config = Context.Config;
            if (Config.Currency.BlackListedChannels.Contains(Channel.Id.ToString()) == true)
            {
                Config.Currency.BlackListedChannels.Remove(Channel.Id.ToString());
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"Users can now gain XP in {Channel.Mention}").Build());
            }
            else if (Config.Currency.BlackListedChannels.Contains(Channel.Id.ToString()) == false || Config.Currency.BlackListedChannels.Count == 0)
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"{Channel.Mention} is not blacklisted.").Build());
        }
        [Command("blacklistedchannels"), Alias("blc")]
        public async Task BlackListedChannelsAsync()
        {
            var Config = Context.Config;
            var Embed = new EmbedBuilder();
            List<ITextChannel> BlackListedChannels = new List<ITextChannel>();
            if (Config.Currency.BlackListedChannels.Count == 0)
            {
                Embed.WithFailedColor();
                Embed.WithAuthor(x =>
                {
                    x.IconUrl = Context.Guild.IconUrl;
                    x.Name = Context.Guild.Name + " blacklisted channels";
                });
                Embed.Description = "No blacklisted channels";
            }
            else
            {
                var Guild = Context.Guild as IGuild;
                foreach (var Channel in Config.Currency.BlackListedChannels)
                {
                    ITextChannel IChannel = await Guild.GetTextChannelAsync(Convert.ToUInt64(Channel)) as ITextChannel;
                    if (IChannel == null)
                        Config.Currency.BlackListedChannels.Remove(Channel);
                    else
                        BlackListedChannels.Add(IChannel);
                }
                Embed.WithSuccesColor();
                Embed.WithAuthor(x =>
                {
                    x.IconUrl = Context.Guild.IconUrl;
                    x.Name = Context.Guild.Name + " blacklisted channels";
                });
                Embed.Description = "In the following channel(s) users can not gain XP:\n" +  string.Join(", ", BlackListedChannels.Select(x => x.Mention));
            }
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
            await Context.Channel.SendMessageAsync("", embed: Embed.Build());
        }
        [Command("leaderboard"), Alias("lb", "top")]
        public async Task LeadboardAsync(int Top = 10)
        {
            if (Top > 50)
                return;
            var Config = Context.Config;
            int Rank = 1;
            var Embed = new EmbedBuilder().WithSuccesColor();
            var toFetchFrom = Config.Currency.UsersList.OrderByDescending(x=>x.Value.TotalKarma).Take(Top);
            var Guild = Context.Guild as IGuild;
            foreach (var User in toFetchFrom)
            {
                var User_ = await Guild.GetUserAsync(Convert.ToUInt64(User.Key)) as IUser;
                Embed.AddField(x => { x.Name = @"\🏆" + $" {Rank++}. {User_.Username}"; x.Value = $"**Level:** {User.Value.Level} || **Total Karma:** {User.Value.TotalKarma}"; });
            }
            await Context.Channel.SendMessageAsync("", embed: Embed.WithTitle($"Leaderboard for {Context.Guild.Name}").WithDescription($"Currently showing Top {Rank-1}\n\n\n").WithThumbnailUrl(Context.Guild.IconUrl).WithFooter(x=> { x.Text = Context.Guild.Name + " leaderboard";
                x.IconUrl = Context.Guild.IconUrl;
            }).Build());
        }
        [Command("rank"), Alias("profile")]
        public async Task RankAsync(SocketGuildUser User = null)
        {
            if (User == null)
                User = Context.User as SocketGuildUser;
                await Context.Channel.SendProfileAsync(null, User);
        }
        [Command("daily"), Alias("credits")]
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
