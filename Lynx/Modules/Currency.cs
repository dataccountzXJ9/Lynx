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
        [Command("equip")]
        public async Task Equip(string BG)
        {
            var Config = Context.Config;
            var UserInfo = Config.Currency.UsersList[Context.User.Id.ToString()];
            if (UserInfo.Backgrounds.Owned.Contains(GetIDByBGName(BG)) == true)
            {
                UserInfo.Backgrounds.EquippedBackground = Convert.ToInt16(GetIDByBGName(BG));
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"You now have {BG.ToUpperInvariant()} equipped.").Build());
                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
            }
            else if(UserInfo.Backgrounds.NotOwned.Contains(GetIDByBGName(BG)) == true)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"You do not own {BG.ToUpperInvariant()}.").Build());
            }
        }
        [Command("shop", RunMode = RunMode.Async)]
        public async Task ShopAsync()
        {
            await Context.Channel.SendShopAsync(Context.User, Context.Client as DiscordSocketClient);           
        }
        [Command("listownedbackgrounds"), Alias("lbg")]
        public async Task ListOwnedBackgrounds()
        {
            var Config = Context.Config.Currency.UsersList[Context.User.Id.ToString()];
            string BGs = null;
            foreach(var BG in Config.Backgrounds.Owned)
            {
                BGs += "• " +  GetNameByBGID(BG) + "\n";
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithDescription(BGs).WithSuccesColor().Build());
        }
        [Command("buy", RunMode = RunMode.Async)]
        public async Task BuyAsync(string Background = null)
        {
            if (Background == null)
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"Background **cannot** be <null>. **Please specify a background**. You can see all the available backgrounds in your personal shop by typing **{DatabaseMethods.GetPrefix(Context.Guild)}shop**").Build());
                return;
            }
            var Config = Context.Config;
            var UserInfo = Config.Currency.UsersList[Context.User.Id.ToString()];
            switch(Background.ToLowerInvariant())
            {
                case "ab":
                    if (UserInfo.Backgrounds.Owned.Contains("8") == false)
                    {
                        if (UserInfo.Credits >= 1350 || UserInfo.Credits > 1350)
                        {
                            UserInfo.Credits -= 1350;
                            UserInfo.Backgrounds.Owned.Add("8");
                            UserInfo.Backgrounds.NotOwned.Remove("8");
                            UserInfo.Backgrounds.EquippedBackground = 8;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Abstract Background, are you sure? (Y/n)").Build());
                            var toReceive =await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Abstact Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Abstact Background").Build());
                    }
                    break;
                case "fb":
                    if (UserInfo.Backgrounds.Owned.Contains("2") == false)
                    {
                        if (UserInfo.Credits >= 2000 || UserInfo.Credits > 2000)
                        {
                            UserInfo.Credits -= 2000;
                            UserInfo.Backgrounds.Owned.Add("2");
                            UserInfo.Backgrounds.NotOwned.Remove("2");
                            UserInfo.Backgrounds.EquippedBackground = 2;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Forest Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Forest Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Forest Background").Build());
                    }
                    break;
                case "ff":
                    if (UserInfo.Backgrounds.Owned.Contains("10") == false)
                    {
                        if (UserInfo.Credits >= 2000 || UserInfo.Credits > 2000)
                        {
                            UserInfo.Credits -= 2000;
                            UserInfo.Backgrounds.Owned.Add("10");
                            UserInfo.Backgrounds.NotOwned.Remove("10");
                            UserInfo.Backgrounds.EquippedBackground = 10;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Flower Field Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Flower Field Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Flower Field Background").Build());

                    }
                        break;
                case "hh":
                    if (UserInfo.Backgrounds.Owned.Contains("3") == false)
                    {
                        if (UserInfo.Credits >= 3500 || UserInfo.Credits > 3500)
                        {
                            UserInfo.Credits -= 3500;
                            UserInfo.Backgrounds.Owned.Add("3");
                            UserInfo.Backgrounds.NotOwned.Remove("3");
                            UserInfo.Backgrounds.EquippedBackground = 3;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Headhunter Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Headhunter Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Headhunter Background").Build());

                    }
                    break;
                case "nh":
                    if (UserInfo.Backgrounds.Owned.Contains("4") == false)
                    {
                        if (UserInfo.Credits >= 3000 || UserInfo.Credits > 3000)
                        {
                            UserInfo.Credits -= 3000;
                            UserInfo.Backgrounds.Owned.Add("4");
                            UserInfo.Backgrounds.NotOwned.Remove("4");
                            UserInfo.Backgrounds.EquippedBackground = 4;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Nighthunter Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Nighthunter Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Nighthunter Background").Build());

                    }
                    break;
                case "pb":
                    if (UserInfo.Backgrounds.Owned.Contains("5") == false)
                    {
                        if (UserInfo.Credits >= 1250 || UserInfo.Credits > 1250)
                        {
                            UserInfo.Credits -= 1250;
                            UserInfo.Backgrounds.Owned.Add("5");
                            UserInfo.Backgrounds.NotOwned.Remove("5");
                            UserInfo.Backgrounds.EquippedBackground = 5;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Pattern Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Pattern Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Pattern Background").Build());

                    }
                    break;
                case "rs":
                    if (UserInfo.Backgrounds.Owned.Contains("7") == false)
                    {

                        if (UserInfo.Credits >= 2000 || UserInfo.Credits > 2000)
                        {
                            UserInfo.Credits -= 2000;
                            UserInfo.Backgrounds.Owned.Add("7");
                            UserInfo.Backgrounds.NotOwned.Remove("7");
                            UserInfo.Backgrounds.EquippedBackground = 7;

                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase RedSky Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy RedSky Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own RedSky Background").Build());

                    }
                    break;
                case "sb":
                    if (UserInfo.Backgrounds.Owned.Contains("11") == false)
                    {
                        if (UserInfo.Credits >= 1750 || UserInfo.Credits > 1750)
                        {
                            UserInfo.Credits -= 1750;
                            UserInfo.Backgrounds.Owned.Add("11");
                            UserInfo.Backgrounds.NotOwned.Remove("11");
                            UserInfo.Backgrounds.EquippedBackground = 11;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Stairs Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Stairs Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own Stairs Background").Build());

                    }
                    break;
                case "sf":
                    if (UserInfo.Backgrounds.Owned.Contains("9") == false)
                    {
                        if (UserInfo.Credits >= 2000 || UserInfo.Credits > 2000)
                        {
                            UserInfo.Credits -= 2000;
                            UserInfo.Backgrounds.Owned.Add("9");
                            UserInfo.Backgrounds.NotOwned.Remove("9");
                            UserInfo.Backgrounds.EquippedBackground = 9;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase SunnyForest Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy SunnyForest Background.");
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You already own SunnyForest Background").Build());

                    }
                    break;
                case "spb":
                    if (UserInfo.Backgrounds.Owned.Contains("6") == false)
                    {
                        if (UserInfo.Credits >= 1500 || UserInfo.Credits > 1500)
                        {
                            UserInfo.Credits -= 1500;
                            UserInfo.Backgrounds.Owned.Add("6");
                            UserInfo.Backgrounds.NotOwned.Remove("6");
                            UserInfo.Backgrounds.EquippedBackground = 6;
                            var toModify = await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("You are about to purchase Space Background, are you sure? (Y/n)").Build());
                            var toReceive = await NextMessageAsync();
                            if (toReceive.Content.ToLowerInvariant() == "y")
                            {
                                await GuildConfig.SaveAsync(Config, Context.Guild.Id);
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"\").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"|").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(@"/").Build());
                                await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithSuccesColor().WithDescription(":credit_card: payment complete.").Build());
                            }
                            else
                                return;
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync($"You do not have enough credits to buy Space Background.");
                        }
                    }
                    break;
            }
        }
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(GuildPermission.ManageGuild)]
        [Command("blacklist"),Alias("bl", "blist")]
        public async Task BlackListChannelAsync(ITextChannel Channel = null)
        {
            if (Channel == null)
                Channel = Context.Channel as ITextChannel;
            var Config = Context.Config;
            Config.Currency.BlackListedChannels.Add(Channel.Id.ToString());
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"I've blacklisted {Channel.Mention}, users can no longer gain XP in {Channel.Mention}").WithFooter(x =>
            {
                x.IconUrl = Context.Guild.IconUrl;
                x.Text = "Blacklisted " + Channel.Name;
            }).Build());
            await GuildConfig.SaveAsync(Config, Context.Guild.Id);
        }
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(GuildPermission.ManageGuild)]
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
                if (User_ == null)
                {
                    Embed.AddField(x => { x.Name = @"\🏆" + $" {Rank++}. {User.Key} - [STATE_USER_LEFT] "; x.Value = $"**Level:** {User.Value.Level} || **Total Karma:** {User.Value.TotalKarma}"; });
                }
                else
                {
                    Embed.AddField(x => { x.Name = @"\🏆" + $" {Rank++}. {User_.Username}"; x.Value = $"**Level:** {User.Value.Level} || **Total Karma:** {User.Value.TotalKarma}"; });
                }
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
        public string GetNameByBGID(string ID)
        {
            switch(ID)
            {
                case "1":
                    return "Default";
                case "2":
                    return "FB";
                case "3":
                    return "HH";
                case "4":
                    return "NH";
                case "5":
                    return "PB";
                case "6":
                    return "SPB";
                case "7":
                    return "RS";
                case "8":
                    return "AB";
                case "9":
                    return "SF";
                case "10":
                    return "FF";
                case "11":
                    return "SB";
                default:
                    return null;
            }
        }
        public string GetIDByBGName(string Name)
        {
            switch (Name.ToUpperInvariant())
            {
                case "DEFAULT":
                    return "1";
                case "FB":
                    return "2";
                case "HH":
                    return "3";
                case "NH":
                    return "4";
                case "PB":
                    return "5";
                case "SPB":
                    return "6";
                case "RS":
                    return "7";
                case "AB":
                    return "8";
                case "SF":
                    return "9";
                case "FF":
                    return "10";
                case "SB":
                    return "11";
                default:
                    return null;
            }
        }
    }
}
