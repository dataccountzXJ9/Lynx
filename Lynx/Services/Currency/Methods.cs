using Discord;
using Discord.WebSocket;
using Lynx.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Lynx.Services.Currency.Images;

namespace Lynx.Services.Currency
{
    public static class Methods
    {
        static GuildConfig GuildConfig = new GuildConfig();
        public static async Task AddToCurrencyList(this SocketUser User)
        {
            var User_ = User as SocketGuildUser;
            var Config = GuildConfig.LoadAsync(User_.Guild.Id);
            Config.Currency.UsersList.TryAdd(User.Id.ToString(), new UserWrapper { });
            await GuildConfig.SaveAsync(Config, User_.Guild.Id);

        }
        public static async Task RemoveFromCurrencyList(this SocketUser User)
        {
            var User_ = User as SocketGuildUser;
            var Config = GuildConfig.LoadAsync(User_.Guild.Id);
            Config.Currency.UsersList.TryRemove(User.Id.ToString(), out UserWrapper Profile);
            await GuildConfig.SaveAsync(Config, User_.Guild.Id);
        }
        public static async Task AwardCurrency(this SocketUser User, IMessageChannel Channel,int Amount)
        {
            var User_ = User as SocketGuildUser;
            var Config = GuildConfig.LoadAsync(User_.Guild.Id);
            var Profile = Config.Currency.UsersList[User.Id.ToString()];
            Profile.Karma += Amount;
            Profile.TotalKarma += Amount;
            if (Profile.Karma > Profile.NeededKarma)
            {
                    Profile.Karma = 0;
                    Profile.TotalKarma += Amount;
                    Profile.Level++;
                    Profile.NeededKarma += 350;
                    await GuildConfig.SaveAsync(Config, User_.Guild.Id);
                System.Drawing.Image LevelUPImage = (Bitmap)System.Drawing.Image.FromFile(GetBackground(User, Background.Level));
                using (Graphics g = Graphics.FromImage(LevelUPImage))
                {
                    System.Drawing.Image AvatarImage = null;
                    if (User.GetAvatarUrl() != null)
                    {
                        await DownloadAvatarAsync(User, 64);
                        AvatarImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Avatar.jpg");
                    }
                    else
                    {
                        AvatarImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/DefaultAvatar.png");
                    }
                    try
                    {
                        System.Drawing.Color myColor = System.Drawing.Color.FromArgb(102, 102, 102);
                        SolidBrush Black = new SolidBrush(myColor);
                        var ResizedAvatarImage = ResizeImage(AvatarImage, 49, 48);
                        g.DrawImage(ResizedAvatarImage, 31, 25);
                        g.DrawString(Profile.Level++.ToString(), new Font("Arial Black", 13), Black, new PointF(48, 85));
                        LevelUPImage.Save("Data/Images/LevelUpOutput.png");
                        await Channel.SendFileAsync("Data/Images/LevelUpOutput.png");
                        return;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
           await GuildConfig.SaveAsync(Config, User_.Guild.Id);
        }
        public static async Task TakeCurrency(this SocketUser User, int Amount)
        {
            var User_ = User as SocketGuildUser;
            var Config = GuildConfig.LoadAsync(User_.Guild.Id);
            Config.Currency.UsersList.TryGetValue(User.Id.ToString(), out UserWrapper Profile);
            Profile.Karma -= Amount;
            Profile.TotalKarma -= Amount;
            Config.Currency.UsersList.AddOrUpdate(User.Id.ToString(), Profile, (str, amnt) => Profile);
            await GuildConfig.SaveAsync(Config, User_.Guild.Id);
        }
        public static async Task<IUserMessage> SendProfileAsync(this IMessageChannel Channel, EmbedBuilder Builder, IUser User)
        {
            System.Drawing.Image BackgroundImage = (Bitmap)System.Drawing.Image.FromFile(GetBackground(User as SocketUser, Background.Profile));
            using (Graphics g = Graphics.FromImage(BackgroundImage))
            {
                var Config = GuildConfig.LoadAsync((User as SocketGuildUser).Guild.Id);
                var Profile = Config.Currency.UsersList[User.Id.ToString()];
                System.Drawing.Color myColor = System.Drawing.Color.FromArgb(53, 53, 53);
                SolidBrush Gray = new SolidBrush(myColor);
                System.Drawing.Image AvatarImage = null;
                if (User.GetAvatarUrl() != null)
                {
                    await DownloadAvatarAsync(User as SocketUser, 64);
                    var AvatarImage_ = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Avatar.jpg");
                    var WhiteBG = Currency.Images.LockBitsImage.Transparent2Color(AvatarImage_, System.Drawing.Color.White);
                    // 63,64
                    AvatarImage = ResizeImage(WhiteBG, 63, 64);
                }
                else
                {
                    var AvatarImage_ = (Bitmap)System.Drawing.Image.FromFile("Data/Images/DefaultAvatar.png");
                    // 64,64
                    AvatarImage = ResizeImage(AvatarImage_, 64, 64);
                }
                //25,18
               
                g.DrawImage(AvatarImage, 25, 18);
                g.DrawString(User.Username, new Font("Arial Black", 10), Gray, new PointF(107, 12));
                var Percentage = Profile.Karma * 100 / Profile.NeededKarma;
                var PercentageImage = new LockBitsImage(new Bitmap(146, 27));
                for (int i = 0; i < Percentage * 1.46; i++)
                {
                    for (int j = 0; j < 21; j++)
                    {
                        PercentageImage.SetPixel(i, j, System.Drawing.Color.DimGray);
                    }
                }
                PercentageImage.Dispose();
                PercentageImage.bmpSource.Save("Data/Images/Percentage.png");
                var PercentageOutput = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Percentage.png");
                // 111,30
                g.DrawImage(PercentageOutput, 111, 30);
                int o = 0;
                int Rank = 0;
                foreach (var User_ in Config.Currency.UsersList.OrderByDescending(x => x.Value.TotalKarma))
                {
                    o++;
                    if (User_.Key == User.Id.ToString())
                    {
                        Rank = o;
                        break;
                    }
                }
                g.DrawString(Profile.Level.ToString(), new Font("Arial Black", 10), Gray, new PointF(123, 65));
                g.DrawString(Rank.ToString(), new Font("Arial Black", 9), Gray, new PointF(219, 52));
                g.DrawString(Profile.Credits.ToString(), new Font("Arial Black", 9), Gray, new PointF(219, 66));
                g.DrawString($"XP: {Profile.Karma} / {Profile.NeededKarma}", new Font("Arial Black", 7), Gray, new PointF(150, 34));
                BackgroundImage.Save("Data/Images/Output.png");
            }
            return await Channel.SendFileAsync("Data/Images/Output.png");
        }
        public static async Task DownloadAvatarAsync(SocketUser User, ushort size)
        {
            var AvatarURI = User.GetAvatarUrl(size: size);
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, AvatarURI))
                {
                    using (
                        Stream contentStream = await(await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream("Data/Images/Avatar.jpg", FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await contentStream.CopyToAsync(stream);
                    }
                }
            }
        }
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static string GetBackground(this SocketUser User, Background Background)
        {
            var Config = GuildConfig.LoadAsync((User as SocketGuildUser).Guild.Id);
            switch (Background)
            {
                case Background.Level:
                    switch (Config.Currency.UsersList[User.Id.ToString()].EquippedLevelBackground)
                    {
                        case 1:
                            return "Data/Images/LevelUPBackgrounds/SpaceUp.png";
                        case 2:
                            return "Data/Images/LevelUPBackgrounds/HeadhunterUp.png";
                        case 3:
                            return "Data/Images/LevelUPBackgrounds/HeadhunterUp.png";
                        case 4:
                            return "Data/Images/LevelUPBackgrounds/ColorfulUp.png";
                        default:
                            return "Data/Images/LevelUPBackgrounds/SpaceUp.png";
                    }
                case Background.Profile:
                    switch (Config.Currency.UsersList[User.Id.ToString()].EquippedBackground)
                    {
                        case 1:
                            return "Data/Images/ProfileBackgrounds/DefaultBackground.png";
                        case 2:
                            return "Data/Images/ProfileBackgrounds/ForestBackground.png";
                        case 3:
                            return "Data/Images/ProfileBackgrounds/HeadhunterBackground.png";
                        case 4:
                            return "Data/Images/ProfileBackgrounds/NighthunterBackground.png";
                        case 5:
                            return "Data/Images/ProfileBackgrounds/PatternBackground.png";
                        case 6:
                            return "Data/Images/ProfileBackgrounds/SpaceBackground.png";
                        default:
                            return "Data/Images/LevelUPBackgrounds/DefaultBackground.png";
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

