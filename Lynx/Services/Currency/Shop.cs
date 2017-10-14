using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Lynx.Database;
using Lynx.Services.Currency;
using System.Linq;
using Lynx.Services.Embed;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
namespace Lynx.Services.Currency
{
    public static class Shop
    { 

        static GuildConfig GuildConfig = new GuildConfig();
        static LynxConfig LynxConfig = new LynxConfig();
        public static async Task<IUserMessage> SendShopAsync(this IMessageChannel Channel, IUser User, DiscordSocketClient Client)
        {
            try
            {
                var toModify = await Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription("Getting your shop ready, whoop whoop.").Build());
                var BotConfig = LynxConfig.LoadConfig;
                var Config = GuildConfig.LoadAsync((Channel as SocketTextChannel).Guild.Id);
                var UserInfo = Config.Currency.UsersList[User.Id.ToString()];
                System.Drawing.Image BackgroundImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Shop/ShopTemplate.png");
                await Currency.Methods.DownloadAvatarAsync(User as SocketUser, 64);
                System.Drawing.Image Avatar = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Avatar.jpg");
                System.Drawing.Image ProfileBackground = (Bitmap)System.Drawing.Image.FromFile(Methods.GetBackground(User as SocketUser, Methods.Background.Profile));
                System.Drawing.Image LevelBackground = (Bitmap)System.Drawing.Image.FromFile(Methods.GetBackground(User as SocketUser, Methods.Background.Level));
                Bitmap resizedAvatar = Methods.ResizeImage(Avatar, 37, 37);
                Bitmap resizedProfileAvatar = Methods.ResizeImage(Avatar, 35, 36);
                Bitmap resizedLevelBackground = Methods.ResizeImage(LevelBackground, 49, 56);
                Bitmap resizedLevelAvatar = Methods.ResizeImage(Avatar, 23, 23);
                using (Graphics g = Graphics.FromImage(BackgroundImage))
                {
                    for (int i = 0; i < UserInfo.Backgrounds.NotOwned.Take(10).Count(); i++)
                    {
                        int Height = 0;
                        int Width = 0;
                        int AHeight = 0;
                        int AWidht = 0;
                        switch (i)
                        {
                            case 0: Height = 18; Width = 16; AHeight = 33; AWidht = 27; break;
                            case 1: Height = 225; Width = 16; AHeight = 240; AWidht = 27; break;
                            case 2: Height = 432; Width = 16; AHeight = 447; AWidht = 27; break;
                            case 3: Height = 18; Width = 92; AHeight = 33; AWidht = 103; break;
                            case 4: Height = 225; Width = 92; AHeight = 240; AWidht = 103; break;
                            case 5: Height = 432; Width = 92; AHeight = 447; AWidht = 103; break;
                            case 6: Height = 18; Width = 166; AHeight = 33; AWidht = 177; break;
                            case 7: Height = 432; Width = 166; AHeight = 447; AWidht = 177; break;
                            case 8: Height = 18; Width = 240; AHeight = 33; AWidht = 251; break;
                            case 9: Height = 432; Width = 240; AHeight = 447; AWidht = 251; break;

                        }

                        Bitmap toResize = (Bitmap)System.Drawing.Image.FromFile(GetBackground(Background.Profile, UserInfo.Backgrounds.NotOwned[i]));
                        Bitmap toDraw = Currency.Methods.ResizeImage(toResize, 183, 64);
                        g.DrawImage(toDraw, Height, Width);
                        g.DrawImage(resizedAvatar, AHeight, AWidht);
                    }
                    var resizedProfileBG = Methods.ResizeImage(ProfileBackground, 173, 61);
                    g.DrawImage(resizedProfileBG, 231, 171);
                    g.DrawImage(resizedProfileAvatar, 245, 181);
                    g.DrawImage(resizedLevelBackground, 232, 230);
                    g.DrawImage(resizedLevelAvatar, 247, 242);
                    await toModify.ModifyAsync(x => x.Embed = new EmbedBuilder().WithDescription("Busy drawing.. :paintbrush: ").WithSuccesColor().Build());
                }
                BackgroundImage.Save("Data/ShopOutput.png");
                Account AccountInfo = new Account
                {
                    ApiKey = BotConfig.Cloudinary.APIKey,
                    ApiSecret = BotConfig.Cloudinary.APISecret,
                    Cloud = BotConfig.Cloudinary.CloudinaryName,

                };
                Cloudinary Cloudinary = new Cloudinary(AccountInfo);
                var UploadParams = new ImageUploadParams
                {
                    File = new FileDescription("Data/ShopOutput.png")
                };
                var UploadResult = Cloudinary.Upload(UploadParams);
                await toModify.DeleteAsync();
                return await Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User.Mention}, here is your shop. Your current credits:** {UserInfo.Credits} :moneybag:\nYou can buy a backgroud by doing " +
    $"**{Lynx.Methods.DatabaseMethods.GetPrefix((Channel as SocketTextChannel).Guild)}buy <#background-id>**. Background ID's & prices are shown on the level progress bar.").WithImageUrl(UploadResult.Uri.ToString()).Build());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return await Channel.SendMessageAsync(e.ToString());
            }
        }
           
        
        
        public static string GetBackground(Background Background, string Case)
        {
            switch (Background)
            {
                case Background.Level:
                    switch (Case)
                    {
                        case "1":
                            return "Data/Images/LevelUPBackgrounds/SpaceUp.png";
                        case "2":
                            return "Data/Images/LevelUPBackgrounds/HeadhunterUp.png";
                        case "3":
                            return "Data/Images/LevelUPBackgrounds/HeadhunterUp.png";
                        case "4":
                            return "Data/Images/LevelUPBackgrounds/ColorfulUp.png";
                        default:
                            return "Data/Images/LevelUPBackgrounds/SpaceUp.png";
                    }
                case Background.Profile:
                    switch (Case)
                    {
                        case "2":
                            return "Data/Images/ProfileBackgrounds/WithPrice/FB.png";
                        case "3":
                            return "Data/Images/ProfileBackgrounds/WithPrice/HH.png";
                        case "4":
                            return "Data/Images/ProfileBackgrounds/WithPrice/NH.png";
                        case "5":
                            return "Data/Images/ProfileBackgrounds/WithPrice/PB.png";
                        case "6":
                            return "Data/Images/ProfileBackgrounds/WithPrice/SPB.png";
                        case "7":
                            return "Data/Images/ProfileBackgrounds/WithPrice/RS.png";
                        case "8":
                            return "Data/Images/ProfileBackgrounds/WithPrice/AB.png";
                        case "9":
                            return "Data/Images/ProfileBackgrounds/WithPrice/SF.png";
                        case "10":
                            return "Data/Images/ProfileBackgrounds/WithPrice/FF.png";
                        case "11":
                            return "Data/Images/ProfileBackgrounds/WithPrice/SB.png";
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }


        public enum Background
        {
            Level,
            Profile
        }
    }

}