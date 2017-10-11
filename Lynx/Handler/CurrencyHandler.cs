using Discord.WebSocket;
using Lynx.Database;
using Sparrow.Collections.LockFree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lynx.Services.Currency;
using NLog;
using System.Threading;

namespace Lynx.Handler
{
    public class CurrencyHandler
    {
        static GuildConfig GuildConfig = new GuildConfig();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static ConcurrentDictionary<string, DateTime> LastCurrency { get; } = new ConcurrentDictionary<string, DateTime>();
        public static Task PotentialCurrency(SocketMessage Message)
        {
            var _ = Task.Run(async () =>
            {
                var Guild = (Message.Channel as SocketTextChannel).Guild;
                var Config = GuildConfig.LoadAsync(Guild.Id);
                if (Config.Currency.IsEnabled == false || Message.Author.IsBot == true || Config.Currency.BlackListedChannels.Contains(Message.Channel.Id.ToString()) == true)
                    return;
                if (Config.Currency.UsersList.ContainsKey(Message.Author.Id.ToString()) == false)
                    await Message.Author.AddToCurrencyList();

                try
                {
                    var Last = LastCurrency.GetOrAdd(Message.Author.Id.ToString(), DateTime.MinValue);
                    if (DateTime.Now - TimeSpan.FromSeconds(60) < Last)
                        return;
                    var RNG = new Random();
                    var Chance = RNG.Next(0, 101) + Config.Currency.Chance * 100;
                    if (Chance > 101)
                    {
                        LastCurrency.AddOrUpdate(Message.Author.Id.ToString(), DateTime.Now, (id, old) => DateTime.Now);
                        await Message.Author.AwardCurrency(Message.Channel, RNG.Next(0, 25));
                    }
                }
                catch (Exception e)
                {
                    logger.Error($"Could not update Currency for [{Message.Author}] in [{Guild.Name}]\n{e.Message}");
                }
            });
            return Task.CompletedTask;

        }
    }
}
    

    
    
    


