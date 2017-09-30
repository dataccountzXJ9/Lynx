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
    }
}