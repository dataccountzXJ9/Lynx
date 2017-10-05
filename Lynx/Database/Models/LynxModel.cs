using System.Collections.Generic;

namespace Lynx.Models.Database
{
    public class LynxModel
    {
        public string BotToken { get; set; }
        public string BotPrefix { get; set; }
        public string ClarifaiAPIKey { get; set; }
        public bool Debug { get; set; }
        public string Id { get; set; }
        public string GoogleAPIKey { get; set; }
        public int MessagesReceived { get; set; }
        public int CommandsTriggered { get; set; }
        public List<string> BotGames { get; set; } = new List<string>();
    }
}
