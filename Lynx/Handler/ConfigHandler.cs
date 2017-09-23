using Discord;
using Discord.WebSocket;
using Lynx.Database;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Threading.Tasks;

namespace Lynx.Handler
{
    public static class ConfigHandler
    {
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);
        public static IDocumentStore Store { get { return store.Value; } }
        public static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                Urls = new string[] { "http://localhost:8080" },
                Database = "Lynx"
            }.Initialize();
            return store;
        }
        public static SConfig LoadServerConfig(this IGuild Guild)
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<SConfig>("GConfigs/" + Guild.Id);
        }
        public static SConfig LoadServerConfig(this SocketGuild Guild)
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<SConfig>("GConfigs/" + Guild.Id);
        }
        public static IBConfig LoadBotConfig(this DiscordSocketClient Client)
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<BConfig>("BotConfig");
        }
        public static IBConfig LoadBotConfigM()
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<BConfig>("BotConfig");
        }
        public static IBConfig LoadBotConfig(this IDiscordClient Client)
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<BConfig>("BotConfig");
        }
        public static IBConfig LoadBotConfig(this IGuild Client)
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<BConfig>("BotConfig");
        }
        public static IBConfig LoadBotConfig(this SocketGuild Client)
        {
            using (IDocumentSession session = Store.OpenSession())
                return session.Load<BConfig>("BotConfig");
        }
        //////////////////////////////////////////////////////////////
        public static Task CheckBotConfig(this DiscordSocketClient Client)
        {
            using (IDocumentSession store = Store.OpenSession())
            {
                var Check = store.Load<BConfig>("BotConfig");
                if (Check == null)
                {
                    store.Store(new BConfig { Id = "BotConfig" });
                }
                store.SaveChanges();
            }
            return Task.CompletedTask;
        }
        public static Task CheckBotConfig(this IDiscordClient Client)
        {
            using (IDocumentSession store = Store.OpenSession())
            {
                var Check = store.Load<BConfig>("BotConfig");
                if (Check == null)
                {
                    store.Store(new BConfig { Id = "BotConfig" });
                }
                store.SaveChanges();
            }
            return Task.CompletedTask;
        }
        public static Task CheckGuildConfig(this SocketGuild Guild)
        {
            using (IDocumentSession store = Store.OpenSession())
            {
                var Check = store.Load<SConfig>("GConfigs/" + Guild.Id);
                if (Check == null)
                {
                    store.Store(new SConfig { Id = "GConfigs/" + Guild.Id.ToString() });
                }
                store.SaveChanges();
            }
            return Task.CompletedTask;
        }
        public static Task CheckGuildConfig(this IGuild Guild)
        {
            using (IDocumentSession store = Store.OpenSession())
            {
                var Check = store.Load<SConfig>("GConfigs/" + Guild.Id);
                if (Check == null)
                {
                    store.Store(new SConfig { Id = "GConfigs/" + Guild.Id.ToString() });
                }
                store.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}
