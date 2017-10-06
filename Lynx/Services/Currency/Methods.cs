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
                System.Drawing.Image LevelUPImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/LevelUPBase.png");
                using (Graphics g = Graphics.FromImage(LevelUPImage))
                {
                    System.Drawing.Image AvatarImage = null;
                    if (User.GetAvatarUrl() != null)
                    {
                        await DownloadAvatarAsync(User, 64);
                        AvatarImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Avatar.png");
                    }
                    else
                    {
                        AvatarImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/DefaultAvatar.png");
                    }
                    try
                    {
                        System.Drawing.Color myColor = System.Drawing.Color.FromArgb(0, 0, 0);
                        SolidBrush Black = new SolidBrush(myColor);
                        var ResizedAvatarImage = ResizeImage(AvatarImage, 55, 53);
                        g.DrawImage(ResizedAvatarImage, 27, 27);
                        g.DrawString(Profile.Level++.ToString(), new Font("Arial Black", 15), Black, new PointF(46, 93));
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
            System.Drawing.Image BackgroundImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/BudgetBackground.png");
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
                    AvatarImage = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Avatar.png");
                }
                else
                {
                    var AvatarImage_ = (Bitmap)System.Drawing.Image.FromFile("Data/Images/DefaultAvatar.png");
                    AvatarImage = ResizeImage(AvatarImage_, 64, 64);
                }
                g.DrawImage(AvatarImage, 16, 14);
                g.DrawString(User.Username, new Font("Arial Black", 10),Gray, new PointF(99, 10));
                var Percentage = Profile.Karma * 100 / Profile.NeededKarma;
                var PercentageImage = new LockBitsImage(new Bitmap(147, 27));
                for (int i = 0; i < Percentage * 1.47; i++)
                {
                    for (int j = 0; j < 21; j++)
                    {
                        PercentageImage.SetPixel(i, j, System.Drawing.Color.DimGray);
                    }
                }
                PercentageImage.Dispose();
                PercentageImage.bmpSource.Save("Data/Images/Percentage.png");
                var PercentageOutput = (Bitmap)System.Drawing.Image.FromFile("Data/Images/Percentage.png");
                g.DrawImage(PercentageOutput, 102, 27);
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
                g.DrawString(Profile.Level.ToString(), new Font("Arial Black", 10), Gray, new PointF(112, 60));
                g.DrawString(Rank.ToString(), new Font("Arial Black", 8), Gray, new PointF(211, 48));
                g.DrawString(Profile.Credits.ToString(), new Font("Arial Black", 8), Gray, new PointF(211, 62));
                g.DrawString($"XP: {Profile.Karma} / {Profile.NeededKarma}", new Font("Arial Black", 7), Gray, new PointF(143, 30));
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
                        stream = new FileStream("Data/Images/Avatar.png", FileMode.Create, FileAccess.Write, FileShare.None))
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

    }
}

