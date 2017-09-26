using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Lynx.Handler;
using Discord.Audio;
using Lynx.Services.Embed;
using Lynx.Services.Music;
using System;
using System.Xml;
using System.Linq;
using Sparrow.Collections.LockFree;

public class AudioModule : ModuleBase
{
    private readonly AudioService _service;
    public AudioModule(AudioService service)
    {
        _service = service;
    }
    [Command("leave", RunMode = RunMode.Async)]
    public async Task LeaveCmd()
    {
        if(AudioService.ConnectedChannels.ContainsKey(Context.Guild.Id))
        {
            await _service.LeaveAudio(Context.Guild);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle("Music Playback").WithDescription($"**Leaving** voice channel.").Build());
        }
        else
        {
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle("Music Playback").WithDescription($"**Not** in a voice channel.").Build());
        }
    }
    [Command("play", RunMode = RunMode.Async)]
    public async Task PlayCmd([Remainder] string song)
    {
        if ((Context.User as IVoiceState).VoiceChannel == null)
        {
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithFailedColor().WithDescription("You **must** be in a voice channel.").Build());
            return;
        }
        if (AudioService.ConnectedChannels.ContainsKey(Context.Guild.Id) == false)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle("Music Playback").WithDescription($"**Joining channel** `{(Context.User as IVoiceState).VoiceChannel.Name}`").Build());
        }
        AudioService.Playing_.TryGetValue(Context.Guild.Id, out bool value);
        if (Youtube.validAuthorities.Any(song.Contains))
        {
            var Response = await Context.Guild.InformationByURLAsync(song);
            await Context.Guild.UpdateSongQueue(UpdateHandler.SongEnum.Add, Response.URL);
            if(value == false)
                await _service.SendAudioAsync(Context.Guild, Context.Channel, Response.URL);
            else
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{Response.YoutubeTitle}** has been queued.").Build());
        }
        else
        {
                var Response = await Context.Guild.GetVideoInformation(song);
                await Context.Guild.UpdateSongQueue(UpdateHandler.SongEnum.Add, Response.URL);
                if(value == false)
                await _service.SendAudioAsync(Context.Guild, Context.Channel, Response.URL);
                else
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{Response.YoutubeTitle}** has been queued.").Build());
        }
    }
}

        
    
