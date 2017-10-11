using Sparrow.Collections.LockFree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
namespace Lynx.Database
{

    public class EventsWrapper
    {
        public bool LogState { get; set; } = false;
        public string LogChannel { get; set; } = "0";
        public EventWrapper Join { get; set; } = new EventWrapper();
        public EventWrapper Leave { get; set; } = new EventWrapper();
        public bool PresenceUpdate { get; set; } = true;
        public bool StatusPresenceUpdate { get; set; } = false;
        public bool UserBan { get; set; } = true;
        public bool UserUnban { get; set; } = true;
        public bool VoicePresenceUpdate { get; set; } = true;
        public bool MessageUpdate { get; set; } = true;
        public bool MessageDelete { get; set; } = true;
        public bool ChannelDelete { get; set; } = true;
        public bool ChannelUpdate { get; set; } = true;
        public bool ChannelCreate { get; set; } = true;
        public bool RoleCreate { get; set; } = true;
        public bool RoleUpdate { get; set; } = true; // todo
        public bool RoleDelete { get; set; } = true;
        public bool NSFWWarning { get; set; } = true;
    }
    public class EmbedB
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string ImageURL { get; set; }
        public uint ColorHex { get; set; }
        public EmbedAuthorBuilder WithAuthor { get; set; } = new EmbedAuthorBuilder();
        public EmbedFooterBuilder WithFooter { get; set; } = new EmbedFooterBuilder();
    }
    public class EmbedAuthorBuilder
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
    }
    public class EmbedFooterBuilder
    {
        public string Text { get; set; }
        public string IconUrl { get; set; }
    }
    public class EventWrapper
    {
        public bool IsEnabled { get; set; }
        public string TextChannel { get; set; } = "0";
    }
    public class ModerationWrapper
    {
        public string MuteRoleID { get; set; } = "0";
        public RoleWrapper DefaultAssignRole { get; set; } = new RoleWrapper();
        public List<string> AssignableRoles { get; set; } = new List<string>();
        public Dictionary<string, MuteWrapper> MuteList { get; set; } = new Dictionary<string, MuteWrapper>();
    }
    public class RoleWrapper
    {
        public bool AutoAssignEnabled { get; set; }
        public string AssignRoleID { get; set; } = "0";
    }
    public class Currency
    {
        public float Chance { get; set; }
        public bool IsEnabled { get; set; }
        public int MaxLevel { get; set; } = 50;
        public List<string> BlackListedChannels { get; set; } = new List<string>();
        public IList<string> LevelUpMessages { get; set; } = new List<string>();
        public IList<ulong> LevelUpRoles { get; set; } = new List<ulong>();
        public ConcurrentDictionary<string, UserWrapper> UsersList { get; set; } = new ConcurrentDictionary<string, UserWrapper>();        
    }
    public class MuteWrapper
    {
        public string GuildId { get; set; }
        public string Reason { get; set; }
        public string ModeratorId { get; set; } = "0";
        public DateTime MutedAt { get; set; }
        public DateTime UnmuteTime { get; set; }
    }
    public class CustomReactionWrapper
    {
        public string Trigger { get; set; }
        public string Response { get; set; }
        public int Id { get; set; }
    }
    public class UserWrapper
    {
        public int Level { get; set; } = 1;
        public int Karma { get; set; } = 0;
        public int TotalKarma { get; set; }
        public int NeededKarma { get; set; } = 1000;
        public int Credits { get; set; }
        public DateTime LastCredit { get; set; }
        public int EquippedBackground { get; set; } = 1;
        public int EquippedLevelBackground { get; set; } = 1;
        public List<string> Backgrounds { get; set; } = new List<string>();
    }
}
    // public List<string> SongList { get; set; } = new List<string>();
    // public Dictionary<string, CustomReactionWrapper> CustomReactions { get; set; } = new Dictionary<string, CustomReactionWrapper>(); VERY WIP

    // public class SongListWrapper
    // {
    //   public string RequestedBy { get; set; } = "0";
    //   public string SongLink { get; set; } = "0";
    // }






