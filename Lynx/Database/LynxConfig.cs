using Lynx.Handler;
using Lynx.Models.Database;
using Raven.Client.Documents.Session;
using System;
using System.Threading.Tasks;

namespace Lynx.Database
{
    public class LynxConfig
    {
        public LynxModel LoadConfig
        {
            get
            {
                using (IDocumentSession Session = ConfigHandler.Store.OpenSession())
                    return Session.Load<LynxModel>("LynxConfig");
            }
        }
        public async Task LoadConfigAsync()
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                if (await Session.LoadAsync<LynxModel>("LynxConfig") == null)
                {
                    Console.WriteLine("IT DOES NOT EXIST");
                    string BotToken = Console.ReadLine();
                    string BotPrefix = Console.ReadLine();
                    await Session.StoreAsync(new LynxModel
                    {
                        Id = "LynxConfig",
                        BotPrefix = BotPrefix,
                        BotToken = BotToken,
                    }).ConfigureAwait(false);
                    
                    await Session.SaveChangesAsync().ConfigureAwait(false); ;
                    Session.Dispose();
                }
                else
                {
                    Console.WriteLine("IT DOES EXIST");
                }
            }
        }
        
        public async Task SaveAsync(LynxModel GetConfig)
        {
            using (IAsyncDocumentSession Session = ConfigHandler.Store.OpenAsyncSession())
            {
                await Session.StoreAsync(GetConfig, id: "LynxConfig").ConfigureAwait(false);
                await Session.SaveChangesAsync().ConfigureAwait(false);
                Session.Dispose();
            }
        }
    }
}
