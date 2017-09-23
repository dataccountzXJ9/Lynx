using Discord.Commands;
using System.Collections.Generic;
using System.Linq;

namespace Lynx.Services.Help
{
    public static class HelpExtension
    {
        public static ModuleInfo GetTopLevelModule(this ModuleInfo module)
        {
            while (module.Parent != null)
            {
                module = module.Parent;
            }
            return module;
        }
        public class CommandTextEqualityComparer : IEqualityComparer<CommandInfo>
        {
            public bool Equals(CommandInfo x, CommandInfo y) => x.Aliases.First() == y.Aliases.First();
            public int GetHashCode(CommandInfo obj) => obj.Aliases.First().GetHashCode();
        }
    }
}
