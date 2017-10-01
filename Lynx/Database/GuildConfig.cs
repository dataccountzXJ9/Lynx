using Discord.WebSocket;
using Lynx.Database.Enums;
using Lynx.Handler;
using Lynx.Models.Database;
using Raven.Client.Documents.Session;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;

namespace Lynx.Database
{
    public class GuildConfig
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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
                        if (await Session.LoadAsync<ServerModel>($"{GuildId}") == null)
                        {
                            var CRW = new List<CustomReactionWrapper>();
                            CRW.Add(new CustomReactionWrapper { Id = 2, Trigger = "test", Response = "-" });
                            CRW.Add(new CustomReactionWrapper { Id = 2, Trigger = "test", Response = "-" });
                            await Session.StoreAsync(
                                new ServerModel
                                {
                                    Id = $"{GuildId}",
                                    ServerPrefix = "?",
                                    CustomReactions = CRW,
                                }).ConfigureAwait(false);
                            logger.Info($"Config [{GuildId}] has been created succesfully.");
                        }
                        break;
                    case Actions.Delete:
                        if (await Session.ExistsAsync($"{GuildId}")) Session.Delete($"{GuildId}"); 
                        logger.Warn($"Config [{GuildId}] has been deleted succesfully."); break;
                }
                await Session.SaveChangesAsync().ConfigureAwait(false);
                Session.Dispose();
            }
        }
    }
}
                
    

