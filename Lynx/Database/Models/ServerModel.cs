using Lynx.Database;
using System.Collections.Generic;

namespace Lynx.Models.Database
{
    public class ServerModel
    {
        public string Id { get; set; }
        public string ServerPrefix { get; set; }
        public bool NSFWFiltering { get; set; } = true;
        public EmbedB WelcomeMessage { get; set; } = new EmbedB();
        public EmbedB LeaveMessage { get; set; } = new EmbedB();
        public EventsWrapper Events { get; set; } = new EventsWrapper();
        public ModerationWrapper Moderation { get; set; } = new ModerationWrapper();
        public List<CustomReactionWrapper> CustomReactions { get; set; } = new List<CustomReactionWrapper>();
        public Currency Currency { get; set; } = new Currency();
    }
}
    

