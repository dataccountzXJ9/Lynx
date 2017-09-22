using System;
using System.Collections.Generic;
using System.Text;

namespace Lynx.Database
{
    public class BConfig : IBConfig
    {
        public string BotToken { get; set; }
        public string BotPrefix { get; set; } = "!";
        public string ClarifaiAPIKey { get; set; }
        public bool Debug { get; set; }
        public string Id { get; set; }
    }
    public interface IBConfig
    {
        string BotPrefix { get; set; }
        string ClarifaiAPIKey { get; set; }
        string BotToken { get; set; }
        bool Debug { get; set; }
        string Id { get; set; }
    }
}
