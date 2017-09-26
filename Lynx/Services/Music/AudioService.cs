using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Lynx.Handler;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Lynx.Database;
using System.Linq;
using static Lynx.Services.Embed.EmbedMethods;
using Lynx.Services.Music;
using System.Threading;
using System;

public class AudioService
{
    public static readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
    public static readonly ConcurrentDictionary<ulong, bool> Playing_ = new ConcurrentDictionary<ulong, bool>();
    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        bool Playing;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            return;
        }
        if (target.Guild.Id != guild.Id)
        {
            return;
        }

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient))
        {
            // If you add a method to log happenings from this service,
            // you can uncomment the following line to make use of that.
            //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
        }
        try
        {
            if (Playing_.TryGetValue(guild.Id, out Playing))
            {
                return;
            }
            var Playing__ = false;
            if (Playing_.TryAdd(guild.Id, Playing__))
            {

            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }
    

    public async Task LeaveAudio(IGuild guild)
    {

        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            Playing_.TryUpdate(guild.Id, false, true);
        }
    }
    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            var URL = "";
            var Guild = (channel as SocketTextChannel).Guild;
            var Config = Guild.LoadServerConfig();
            Playing_.TryUpdate(Guild.Id, true, false);
            if (Lynx.Services.Music.Youtube.validAuthorities.Any(path.Contains))
            {
                var Information = await Guild.InformationByURLAsync(path);
                URL = Information.URL;
                await (channel as SocketTextChannel).SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle("Music Playback").WithDescription($"Now playing [{Information.YoutubeTitle}]({Information.URL})").WithThumbnailUrl(Information.ThumbnailURL).Build());
            }
            else
            {
                URL = path;
                var Information = await Guild.GetVideoInformation(path);
                await (channel as SocketTextChannel).SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle("Music Playback").WithDescription($"Now playing [{Information.YoutubeTitle}]({Information.URL})").WithThumbnailUrl(Information.ThumbnailURL).Build());
            }
            await Guild.UpdateSongQueue(UpdateHandler.SongEnum.Remove, URL);
            var output = CreateStream(path).StandardOutput.BaseStream;
            var stream = client.CreatePCMStream(AudioApplication.Music);
            await output.CopyToAsync(stream,81920);
            await stream.FlushAsync().ConfigureAwait(false);
            var Config2 = Guild.LoadServerConfig();
            if ((guild as SocketGuild).CurrentUser.VoiceChannel.Users.Count == 1)
            {
                await LeaveAudio(guild);
                Playing_.TryUpdate(Guild.Id,false,true);
            }
            if (Config2.SongList.Count == 0)
            {
                await LeaveAudio(guild);
                Playing_.TryUpdate(Guild.Id, false,true);
            }
            else
            {
                Playing_.TryUpdate(Guild.Id, false,true);
                var Config3 = Guild.LoadServerConfig();
                await Guild.UpdateSongQueue(UpdateHandler.SongEnum.Remove, Config.SongList[0]);
                await SendAudioAsync(guild, channel, Config2.SongList[0]);
                if (Config3.SongList.Count == 0)
                {
                    await LeaveAudio(guild);
                    Playing_.TryUpdate(Guild.Id, false,true);
                }
            }
        }
    }
    private Process CreateStream(string url)
    {
        Process currentsong = new Process();

        currentsong.StartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C youtube-dl --audio-format best -o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = false
        };
        currentsong.Start();
        return currentsong;
    }
}
