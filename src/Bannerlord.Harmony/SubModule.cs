using TaleWorlds.MountAndBlade;

namespace Bannerlord.Harmony
{
    public class SubModule : MBSubModuleBase
    {
        private readonly HarmonyLib.Harmony? _harmony;

        public SubModule()
        {
            _harmony = null;
        }
    }
}