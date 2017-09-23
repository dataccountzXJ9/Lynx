using Discord;
using Discord.WebSocket;
using Lynx.Handler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static NSFWModels;

namespace NSFW
{
    public class NSFWService
    {
        private static async Task<bool> IsImageUrlAsync(string URL)
        {
            Uri targetUriA = null;
            Uri targetUriB = null;
            try
            {
                if (Uri.TryCreate(URL, UriKind.RelativeOrAbsolute, out targetUriA))
                {
                    var req = (HttpWebRequest)WebRequest.Create(URL);
                    req.Method = "HEAD";
                    var resp = await req.GetResponseAsync();
                    return resp.ContentType.ToLowerInvariant().StartsWith("image/");
                }
            }
            catch
            {
                try
                {
                    if (Uri.TryCreate(@"http://" + URL, UriKind.RelativeOrAbsolute, out targetUriB))
                    {
                        var req = (HttpWebRequest)WebRequest.Create(@"http://" + URL);
                        req.Method = "HEAD";
                        var resp = await req.GetResponseAsync();
                        return resp.ContentType.ToLowerInvariant().StartsWith("image/");
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        public static string ClarifaiTagging(string url)
        {
            const string CLARIFAI_API_URL = "https://api.clarifai.com/v2/models/e9576d86d2004ed1a38ba0cf39ecb4b1/outputs";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigHandler.LoadBotConfigM().ClarifaiAPIKey);

                HttpContent json = new StringContent(
                    "{" +
                        "\"inputs\": [" +
                            "{" +
                                "\"data\": {" +
                                    "\"image\": {" +
                                        "\"url\": \"" + url + "\"" +
                                    "}" +
                               "}" +
                            "}" +
                        "]" +
                    "}", Encoding.UTF8, "application/json");

                var response = client.PostAsync(CLARIFAI_API_URL, json).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string jsonX = response.Content.ReadAsStringAsync().Result.ToString();
                NSFWModel model = JsonConvert.DeserializeObject<NSFWModel>(jsonX);

                var nsfwval = model.outputs.FirstOrDefault().data.concepts.Find(t => t.name == "nsfw").value;
                if (nsfwval > 0.75)
                {
                    return "nsfw";
                }

                return "sfw";
            }
        }

        private static async Task<bool> NSFWFiltered(IGuild guild, SocketUserMessage usrMsg)
        {
            if ((usrMsg.Channel as SocketTextChannel).IsNsfw) return false;

            var message = usrMsg.Content;
            var embeds = usrMsg.Embeds;
            var attatchs = usrMsg.Attachments;
            if (embeds != null)
            {
                foreach (var embed in embeds)
                {
                    if (embed.Type == EmbedType.Image)
                    {
                        var tag = ClarifaiTagging(embed.Url);

                        if (tag != null)
                        {
                            if (tag == "nsfw")
                            {
                                try
                                {
                                    await usrMsg.DeleteAsync();
                                }
                                catch { }
                                return true;
                            }
                        }
                        else
                        {
                            Console.WriteLine("null error A");
                        }
                    }
                }
            }

            if (attatchs != null)
            {
                foreach (var attatch in attatchs)
                {
                    var tag = ClarifaiTagging(attatch.Url);

                    if (tag != null)
                    {
                        if (tag == "nsfw")
                        {
                            try
                            {
                                await usrMsg.DeleteAsync().ConfigureAwait(false);
                            }
                            catch { }
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("null error B");
                    }
                }
            }

            //Finally check raw post URLs (does not support HTTPS)
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match m in linkParser.Matches(message))
            {
                bool ret = await IsImageUrlAsync(m.Value);
                if (ret == true)
                {
                    var tag = ClarifaiTagging(m.Value);

                    if (tag != null)
                    {
                        if (tag == "nsfw")
                        {
                            try
                            {
                                await usrMsg.DeleteAsync();
                            }
                            catch { }
                            return true;
                        }
                    }
                }
                else if (ret == false)
                {
                    bool ret1 = await IsImageUrlAsync(m.Value + ".png");
                    if (ret1 == true)
                    {
                        var tag = ClarifaiTagging(m.Value + ".png");
                        if (tag == "nsfw")
                        {
                            try
                            {
                                await usrMsg.DeleteAsync();
                            }
                            catch { }
                            return true;
                        }
                    }
                    else
                    {
                        bool ret2 = await IsImageUrlAsync(m.Value + ".jpg");
                        if (ret2 == true)
                        {
                            var tag = ClarifaiTagging(m.Value + ".jpg");
                            if (tag == "nsfw")
                            {
                                try
                                {
                                    await usrMsg.DeleteAsync();
                                }
                                catch { }
                                return true;
                            }
                        }

                    }
                }
            }

            return false;
        }
        public static Task NSFWImplementation(SocketMessage message)
        {
            Task.Run(async () =>
            {
                var guild = (message.Channel as SocketTextChannel).Guild;
                await NSFWFiltered(guild, message as SocketUserMessage);
            });
            return Task.CompletedTask;
        }
    }
}
    

        
    
public class NSFWModels

{
    public class Status
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class Status2
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class OutputInfo
    {
        public string message { get; set; }
        public string type { get; set; }
    }

    public class Status3
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class ModelVersion
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public Status3 status { get; set; }
    }

    public class Model
    {
        public string name { get; set; }
        public string id { get; set; }
        public string created_at { get; set; }
        public object app_id { get; set; }
        public OutputInfo output_info { get; set; }
        public ModelVersion model_version { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }

    public class Data
    {
        public Image image { get; set; }
    }

    public class Input
    {
        public string id { get; set; }
        public Data data { get; set; }
    }

    public class Concept
    {
        public string id { get; set; }
        public string name { get; set; }
        public object app_id { get; set; }
        public double value { get; set; }
    }

    public class Data2
    {
        public List<Concept> concepts { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public Status2 status { get; set; }
        public string created_at { get; set; }
        public Model model { get; set; }
        public Input input { get; set; }
        public Data2 data { get; set; }
    }

    public class NSFWModel
    {
        public Status status { get; set; }
        public List<Output> outputs { get; set; }
    }
}