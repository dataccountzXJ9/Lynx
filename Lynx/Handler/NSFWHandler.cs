﻿using Discord;
using Discord.WebSocket;
using Lynx.Database;
using Lynx.Handler;
using Lynx.Methods;
using Lynx.Services.Embed;
using Newtonsoft.Json;
using NLog;
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
        static LynxConfig LynxConfig = new LynxConfig();
        static GuildConfig GuildConfig = new GuildConfig();
        static Logger logger = LogManager.GetCurrentClassLogger();
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
            try
            {
                const string CLARIFAI_API_URL = "https://api.clarifai.com/v2/models/e9576d86d2004ed1a38ba0cf39ecb4b1/outputs";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Key " + LynxConfig.LoadConfig.ClarifaiAPIKey);

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
                        Console.WriteLine(response.StatusCode);
                        return null;
                    }

                    string jsonX = response.Content.ReadAsStringAsync().Result.ToString();
                    NSFWModel model = JsonConvert.DeserializeObject<NSFWModel>(jsonX);

                    var nsfwval = model.outputs.FirstOrDefault().data.concepts.Find(t => t.name == "nsfw").value;
                    if (nsfwval > 0.85)
                    {
                        return "nsfw";
                    }

                    return "sfw";
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return "sfw";
            }
        }

        private static async Task<bool> NSFWFiltered(IGuild guild, SocketUserMessage usrMsg)
        {
            if ((usrMsg.Author.IsBot)) return false;
                if ((usrMsg.Channel as SocketTextChannel).IsNsfw) return false;
                var Guild = (usrMsg.Channel as SocketTextChannel).Guild;
                var Config = GuildConfig.LoadAsync(Guild.Id);
                if (Config.NSFWFiltering == false)
                    return false;
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
                                        if (Config.Events.NSFWWarning == true && Config.Events.LogChannel != "0" && Config.Events.LogState == true)
                                            await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.NSFWLog(usrMsg).Build());
                                        EventsHandler.NSFWDeleted = true;
                                        await usrMsg.DeleteAsync();
                                        await usrMsg.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"{usrMsg.Author.Mention} please do not post nsfw.").Build());
                                        logger.Warn($"NSFW Image has been deleted in [{guild.Id} - {guild.Name}] by [{usrMsg.Author} - {usrMsg.Author.Id}]");
                                    }
                                    catch (Exception e) { logger.Error(e.Message); }
                                    return true;
                                }
                            }
                            else
                            {
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
                                    if (Config.Events.NSFWWarning == true && Config.Events.LogChannel != "0" && Config.Events.LogState == true)
                                        await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.NSFWLog(usrMsg).Build());
                                    EventsHandler.NSFWDeleted = true;
                                    await usrMsg.DeleteAsync();
                                    await usrMsg.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"{usrMsg.Author.Mention} please do not post nsfw.").Build());
                                    logger.Warn($"NSFW Image has been deleted in [{guild.Id} - {guild.Name}] by [{usrMsg.Author} - {usrMsg.Author.Id}]");
                                }
                                catch (Exception e) { logger.Error(e.Message); }
                                return true;
                            }
                        }
                        else
                        {
                        }
                    }
                }
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
                                    if (Config.Events.NSFWWarning == true && Config.Events.LogChannel != "0" && Config.Events.LogState == true)
                                        await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.NSFWLog(usrMsg).Build());
                                    EventsHandler.NSFWDeleted = true;
                                    await usrMsg.DeleteAsync();
                                    await usrMsg.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"{usrMsg.Author.Mention} please do not post nsfw.").Build());
                                    logger.Warn($"NSFW Image has been deleted in [{guild.Id} - {guild.Name}] by [{usrMsg.Author} - {usrMsg.Author.Id}]");
                                }
                                catch (Exception e) { logger.Error(e.Message); }
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
                                    if (Config.Events.NSFWWarning == true && Config.Events.LogChannel != "0" && Config.Events.LogState == true)
                                        await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.NSFWLog(usrMsg).Build());
                                    EventsHandler.NSFWDeleted = true;
                                    await usrMsg.DeleteAsync();
                                    await usrMsg.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"{usrMsg.Author.Mention} please do not post nsfw.").Build());
                                    logger.Warn($"NSFW Image has been deleted in [{guild.Id} - {guild.Name}] by [{usrMsg.Author} - {usrMsg.Author.Id}]");
                                }
                                catch (Exception e) { logger.Error(e.Message); }
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
                                        if (Config.Events.NSFWWarning == true && Config.Events.LogChannel != "0" && Config.Events.LogState == true)
                                            await Guild.GetLogChannel().SendMessageAsync("", embed: EmbedMethods.NSFWLog(usrMsg).Build());
                                        EventsHandler.NSFWDeleted = true;
                                        await usrMsg.DeleteAsync();
                                        await usrMsg.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription($"{usrMsg.Author.Mention} please do not post nsfw.").Build());
                                        logger.Warn($"NSFW Image has been deleted in [{guild.Id} - {guild.Name}] by [{usrMsg.Author} - {usrMsg.Author.Id}]");
                                    }
                                    catch (Exception e) { logger.Error(e.Message); }
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
            var _ = Task.Run(async () =>
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