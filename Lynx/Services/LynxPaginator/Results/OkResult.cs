using Discord.Commands;
namespace Lynx.Interactive
{
    public class OkResult : RuntimeResult
    {
        public OkResult(string reason = null) : base(null, reason) { }
    }
}