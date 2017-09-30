namespace Lynx.Models.Database
{
    public class LynxModel
    {
        public string BotToken { get; set; }
        public string BotPrefix { get; set; } = "!";
        public string ClarifaiAPIKey { get; set; }
        public bool Debug { get; set; }
        public string Id { get; set; }
        public string GoogleAPIKey { get; set; }
        public int MessagesReceived { get; set; } = 0;
    }
}
