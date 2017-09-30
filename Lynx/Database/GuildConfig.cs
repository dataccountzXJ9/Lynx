using Discord.WebSocket;
using Lynx.Database.Enums;
using Lynx.Handler;
using Lynx.Models.Database;
using Raven.Client.Documents.Session;
using System.Threading.Tasks;

namespace Lynx.Database
{
    public class GuildConfig
    {
        public ServerModel LoadAsync(ulong GuildId)
        {
                using (IDocumentSession Session = ConfigHandler.Store.OpenSession())
                    return Session.Load<ServerModel>($"{GuildId}");
        }
        public async Task SaveAsync(ServerModel Model, ulong GuildId)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                await Session.StoreAsync(Model, id: $"{GuildId}").ConfigureAwait(false);
                await Session.SaveChangesAsync().ConfigureAwait(false);
                Session.Dispose();
            }
        }
        public static async Task LoadOrDeleteAsync(Actions Action, ulong GuildId)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                switch (Action)
                {
                    case Actions.Add:
                        if (!await Session.ExistsAsync($"{GuildId}"))
                            await Session.StoreAsync(
                                new ServerModel
                                {
                                    Id = $"{GuildId}",
                                    ServerPrefix = "?>"
                                }).ConfigureAwait(false);
                        break;
                    case Actions.Delete:
                        if (await Session.ExistsAsync($"{GuildId}")) Session.Delete($"{GuildId}"); break;
                }
                await Session.SaveChangesAsync().ConfigureAwait(false);
                Session.Dispose();
            }
        }
    }
}
                
    

