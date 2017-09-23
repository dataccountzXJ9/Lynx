using Discord;
using Discord.WebSocket;
using System.Text;

namespace Lynx.Services.Embed
{
    public static class Placeholders
    {
        public static string GetPlaceholder(string Input, IUser User)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("%usermention%", User.Mention);
            sb.Replace("%username%", User.Username);
            sb.Replace("%useravatar%", User.GetAvatarUrl());
            sb.Replace("%servername%", (User as SocketGuildUser).Guild.Name);
            sb.Replace("%servericon%", (User as SocketGuildUser).Guild.IconUrl);
            return sb.ToString();
        }
    }
}
