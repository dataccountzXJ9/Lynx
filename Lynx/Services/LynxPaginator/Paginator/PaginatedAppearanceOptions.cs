using Discord;
using System;

namespace Lynx.Interactive
{
    public class PaginatedAppearanceOptions
    {
        public static PaginatedAppearanceOptions Default = new PaginatedAppearanceOptions();
        public IEmote Back = new Emoji("◀");
        public IEmote Next = new Emoji("▶");
        public string FooterFormat = "Page {0}/{1}";
        public TimeSpan? Timeout = null;
        public TimeSpan InfoTimeout = TimeSpan.FromSeconds(30);
    }

    public enum JumpDisplayOptions
    {
        Never,
        WithManageMessages,
        Always
    }
}