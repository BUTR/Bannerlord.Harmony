using TaleWorlds.Library;

namespace Bannerlord.BUTRLoader.ModuleInfoExtended
{
    internal readonly struct DependedModuleMetadata
    {
        public string Id { get; init; }
        public LoadType LoadType { get; init; }
        public bool IsOptional { get; init; }
        public bool IsIncompatible { get; init; }
        public ApplicationVersion Version { get; init; }

        internal static string GetLoadType(LoadType loadType) => loadType switch
        {
            LoadType.NONE           => "",
            LoadType.LoadAfterThis  => "Before       ",
            LoadType.LoadBeforeThis => "After        ",
            _                       => "ERROR        "
        };
        private static string GetVersion(ApplicationVersion av) => av.IsSame(ApplicationVersion.Empty) ? "" : $" {av}";
        private static string GetOptional(bool isOptional) => isOptional ? " Optional" : "";
        private static string GetIncompatible(bool isOptional) => isOptional ? "Incompatible " : "";
        public override string ToString() => GetLoadType(LoadType) + GetIncompatible(IsIncompatible) + Id + GetVersion(Version) + GetOptional(IsOptional);
    }
}