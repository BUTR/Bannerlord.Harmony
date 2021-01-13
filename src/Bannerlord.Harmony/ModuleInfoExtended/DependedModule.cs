using TaleWorlds.Library;

namespace Bannerlord.BUTRLoader.ModuleInfoExtended
{
    internal readonly struct DependedModule
    {
        public string ModuleId { get; init; }
        public ApplicationVersion Version { get; init; }
    }
}