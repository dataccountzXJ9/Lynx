using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;
using Discord;
using Lynx.Handler;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Lynx.Services.Music
{
    public static class Youtube
    {
        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private static Regex regexExtractId = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
        public static string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };

        public class YoutubeResponse
        {
            public string YoutubeTitle { get; set; }
            public string Channel { get; set; }
            public string Duration { get; set; }
            public string ViewCount { get; set; }
            public string Likes { get; set; }
            public string Dislikes { get; set; }
            public string URL { get; set; }
            public string ThumbnailURL { get; set; }
        }
        public static string ExtractVideoIdFromUri(Uri uri)
        {
            try
            {
                string authority = new UriBuilder(uri).Uri.Authority.ToLower();
                if (validAuthorities.Contains(authority))
                {
                    var regRes = regexExtractId.Match(uri.ToString());
                    if (regRes.Success)
                    {
                        return regRes.Groups[1].Value;
                    }
                }
            }
            catch { }
            return null;
        }
        public static async Task<YoutubeResponse> InformationByURLAsync(this IGuild Guild, string URL)
        {
            var Response = new YoutubeResponse();
            using (var http = new HttpClient())
            {

                try
                {
                    var Info = JObject.Parse(await http.GetStringAsync($"https://www.googleapis.com/youtube/v3/videos?id=" + ExtractVideoIdFromUri(new System.Uri(URL)) +"&part=snippet,contentDetails,statistics&key=AIzaSyDhjVUrx2_YJuVdkkgjq2Cvn4Pw9DOW-9M"));
                    var CInfo = JObject.Parse(await http.GetStringAsync($"https://www.googleapis.com/youtube/v3/channels?part=snippet&id=" + Info["items"][0]["snippet"]["channelId"] + "&key=AIzaSyDhjVUrx2_YJuVdkkgjq2Cvn4Pw9DOW-9M"));
                    Response.YoutubeTitle = Info["items"][0]["snippet"]["title"].ToString();
                    Response.URL = "https://www.youtube.com/watch?v=" + ExtractVideoIdFromUri(new System.Uri(URL));
                    Response.Likes = Info["items"][0]["statistics"]["likeCount"].ToString();
                    Response.Dislikes = Info["items"][0]["statistics"]["dislikeCount"].ToString();
                    Response.ViewCount = Info["items"][0]["statistics"]["viewCount"].ToString() ?? null;
                    Response.Duration = Info["items"][0]["contentDetails"]["duration"].ToString();
                    Response.Channel = CInfo["items"][0]["snippet"]["title"].ToString();
                    Response.ThumbnailURL = Info["items"][0]["snippet"]["thumbnails"]["default"]["url"].ToString();
                    return Response;
                }
                catch
                {

                }
            }
            return null;
        }
        public static async Task LookupVideos(this IGuild Guild, string VideoURLorName)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Guild.LoadBotConfig().GoogleAPIKey,
                ApplicationName = typeof(Youtube).ToString()
            });
            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = VideoURLorName;
            searchRequest.MaxResults = 1;
            var ServiceReponse = searchRequest.ExecuteAsync();
            foreach (var Video in ServiceReponse.Result.Items.Take(1))
            {
                switch (Video.Id.Kind)
                {
                    case "youtube#video":
                        await Guild.UpdateSongQueue(UpdateHandler.SongEnum.Add, $"https://www.youtube.com/watch?v={Video.Id.VideoId}"); break;
                }
            }
        }
        public static async Task<YoutubeResponse> GetVideoInformation(this IGuild Guild, string URL)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Guild.LoadBotConfig().GoogleAPIKey,
                ApplicationName = typeof(Youtube).ToString()
            });
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = URL;
            var searchListResponse = await searchListRequest.ExecuteAsync();
            YoutubeResponse Response = new YoutubeResponse();
            foreach (var searchResult in searchListResponse.Items.Take(1))
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        Response.ThumbnailURL = searchResult.Snippet.Thumbnails.Default__.ToString();
                        Response.URL = $"https://www.youtube.com/watch?v={searchResult.Id.VideoId}";
                        Response.Channel = searchResult.Snippet.ChannelTitle;
                        Response.YoutubeTitle = searchResult.Snippet.Title; break;
                }
            }
            return Response;
        }
    }
}
       
    

