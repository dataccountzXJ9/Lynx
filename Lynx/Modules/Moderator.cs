using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Threading;
using Lynx.Database;
using System.Linq;
using Lynx.Methods;
using Lynx.Handler;
using Lynx.Services.Embed;

namespace Lynx.Modules
{
    [RequireUserPermission(GuildPermission.ManageGuild), RequireBotPermission(GuildPermission.ManageGuild | GuildPermission.SendMessages)]
    public class Moderator : ModuleBase
    {
        OverwritePermissions MutePermissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        [Command("unmute")]
        public async Task UnmuteAsync(IUser User)
        {
            var Config = Context.Guild.LoadServerConfig();
            if (Context.Client.LoadMuteList().MuteList.ContainsKey(User.Id.ToString()))
            {
                IRole MuteRole = Context.Guild.GetRole(Convert.ToUInt64(Config.Moderation.MuteRoleID)) as IRole;
                await Context.Client.UpdateMuteList(User, null, UpdateHandler.MuteOption.Unmute);
                await (User as SocketGuildUser).RemoveRoleAsync(MuteRole);
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** has been **unmuted** from and text chat.").Build());
                return;
            }
            else
            {
                await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithDescription($"**{User}** is not muted.").Build());
            }
        }
        [Command("mute")]
        public async Task MuteAsync(IUser User, int value = 1, string time = "hour", [Remainder] string reason = "No reason has been provided.")
        {
            var UnmuteTime = Services.Mute.Extensions.GetTime(value, time);
            var Config = Context.Guild.LoadServerConfig();
            if (Config.Moderation.MuteRoleID == null || Config.Moderation.MuteRoleID == "0")
            {
                var Role = await Context.Guild.CreateRoleAsync("Lynx-Mute", GuildPermissions.None, Color.DarkOrange) as IRole;
                foreach (var TextChannel in (Context.Guild as SocketGuild).TextChannels)
                {
                    try
                    {
                        await TextChannel.AddPermissionOverwriteAsync(Role, MutePermissions).ConfigureAwait(false);
                    } catch { }
                }
                await Context.Guild.UpdateServerModeration(UpdateHandler.Moderation.MuteRole, Role.Id);
                await Context.Client.UpdateMuteList(User,Context.User, UpdateHandler.MuteOption.Mute, UnmuteTime, reason);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            else if(Config.Moderation.MuteRoleID != null || Config.Moderation.MuteRoleID != "0")
            {
                
                var Role = Context.Guild.GetRole(Convert.ToUInt64(Context.Guild.LoadServerConfig().Moderation.MuteRoleID)) as IRole;
                foreach (var TextChannel in (Context.Guild as SocketGuild).TextChannels)
                {
                    if (TextChannel.PermissionOverwrites.Select(x => x.Permissions).Contains(MutePermissions) == false)
                    {
                        try
                        {
                            await TextChannel.AddPermissionOverwriteAsync(Role, MutePermissions).ConfigureAwait(false);
                        }
                        catch { }
                    }
                }       
                await Context.Client.UpdateMuteList(User, Context.User,UpdateHandler.MuteOption.Mute, UnmuteTime, reason);
                await (User as SocketGuildUser).AddRoleAsync(Role);
            }
            await Context.Channel.SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle(":zipper_mouth: User has been muted.").WithDescription(
                $"{User} has been succesfully muted.\n\n**User:** {User}\n**Unmute Date:** {UnmuteTime.ToLocalTime()}\n**Reason:** {reason}").Build());
            await Context.Guild.GetLogChannel().SendMessageAsync("", embed: new EmbedBuilder().WithSuccesColor().WithTitle(":zipper_mouth: User has been muted.").WithDescription(
            $"{User} has been succesfully muted.\n\n**User:** {User}\n**Unmute Date:** {UnmuteTime.ToLocalTime()}\n**Reason:** {reason}").Build());
        }
    }
}
        
    

        
   



