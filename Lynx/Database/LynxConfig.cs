using Lynx.Handler;
using Lynx.Models.Database;
using NLog;
using Raven.Client.Documents.Session;
using System;
using System.Threading.Tasks;

namespace Lynx.Database
{
    public class LynxConfig
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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
                    logger.Error("No bot config has been found. Creating one..");
                    logger.Error("Input bot token: ");
                    string BotToken = Console.ReadLine();
                    logger.Error("Input bot prefix: ");
                    string BotPrefix = Console.ReadLine();
                    await Session.StoreAsync(new LynxModel
                    {
                        Id = "LynxConfig",
                        BotPrefix = BotPrefix,
                        BotToken = BotToken,
                    }).ConfigureAwait(false);
                    await Session.SaveChangesAsync().ConfigureAwait(false);
                    logger.Log(LogLevel.Info, "Bot config has been created.");
                    Session.Dispose();
                }
                else
                {
                    logger.Log(LogLevel.Info, "Bot config has been succesfully loaded.");
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
