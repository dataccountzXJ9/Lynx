using System;

namespace Lynx.Services
{
    public class Log
    {
        static void AppendLine(string Text, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            Console.Write(Text);
        }
        public static void CLog(Source Source, string Text)
        {
            Console.Write(Environment.NewLine);
            switch (Source)
            {
                case Source.Database: AppendLine($"[{Source}]", ConsoleColor.Red); break;
                case Source.CommandHandler: AppendLine($"[{Source}]", ConsoleColor.Red); break;
                case Source.NSFWHandler: AppendLine($"[{Source}]", ConsoleColor.Red); break;
                case Source.CustomReactionHandler: AppendLine($"[{Source}]", ConsoleColor.Red); break;
                case Source.Client: AppendLine($"[{Source}]", ConsoleColor.Red); break;
            }
            AppendLine($" {Text}", ConsoleColor.Gray);
        }
    }
    public enum Source
    {
        Client,
        Database,
        CommandHandler,
        NSFWHandler,
        CustomReactionHandler
    }
}
   

