using System;

namespace Bannerlord.Harmony.Utils
{
    public class HarmonyVersionAttribute : Attribute
    {
        public Version Version { get; }

        public HarmonyVersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }
    }
}