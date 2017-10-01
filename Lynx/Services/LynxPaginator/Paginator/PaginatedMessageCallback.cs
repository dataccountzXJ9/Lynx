using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Lynx.Handler;

namespace Lynx.Interactive
{
    public class PaginatedMessageCallback : IReactionCallback
    {
        public LynxContext Context { get; }
        public InteractiveService Interactive { get; private set; }
        public IUserMessage Message { get; private set; }
        public RunMode RunMode => RunMode.Sync;
        public ICriterion<SocketReaction> Criterion => _criterion;
        public TimeSpan? Timeout => options.Timeout;
        private readonly ICriterion<SocketReaction> _criterion;
        private readonly PaginatedMessage _pager;
        private PaginatedAppearanceOptions options => _pager.Options;
        private readonly int pages;
        private int page = 1;
        public PaginatedMessageCallback(InteractiveService interactive,
            LynxContext sourceContext,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            Interactive = interactive;
            Context = sourceContext;
            _criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            _pager = pager;
            pages = _pager.Pages.Count();
        }

        public async Task DisplayAsync()
        {
            var embed = BuildEmbed();
            var message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed).ConfigureAwait(false);
            Message = message;
            Interactive.AddReactionCallback(message, this);
            _ = Task.Run(async () =>
            {
                await message.AddReactionAsync(options.Back);
                await message.AddReactionAsync(options.Next);
            });
            if (Timeout.HasValue && Timeout.Value != null)
            {
                _ = Task.Delay(Timeout.Value).ContinueWith(_ =>
                {
                    Interactive.RemoveReactionCallback(message);
                    _ = Message.DeleteAsync();
                });
            }
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;
            if (emote.Equals(options.Next))
            {
                if (page >= pages)
                    return false;
                ++page;
            }
            else if (emote.Equals(options.Back))
            {
                if (page <= 1)
                    return false;
                --page;
            }
            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync().ConfigureAwait(false);
            return false;
        }

        protected Embed BuildEmbed()
        {
            return new EmbedBuilder()
                .WithAuthor(_pager.Author)
                .WithColor(_pager.Color)
                .WithDescription(_pager.Pages.ElementAt(page - 1).ToString())
                .WithFooter(f => f.Text = string.Format(options.FooterFormat, page, pages))
                .WithTitle(_pager.Title)
                .Build();
        }
        private async Task RenderAsync()
        {
            var embed = BuildEmbed();
            await Message.ModifyAsync(m => m.Embed = embed).ConfigureAwait(false);
        }
    }
}