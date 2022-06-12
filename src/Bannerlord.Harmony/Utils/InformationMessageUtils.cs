using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using TaleWorlds.Library;

namespace Bannerlord.Harmony.Utils
{
    internal static class InformationMessageUtils
    {
        private delegate object V1Delegate(string information, Color color);
        private static readonly V1Delegate? V1;

        static InformationMessageUtils()
        {
            var type = AccessTools2.TypeByName("TaleWorlds.Core.InformationMessage") ??
                       AccessTools2.TypeByName("TaleWorlds.Library.InformationMessage");
            foreach (var constructorInfo in AccessTools.GetDeclaredConstructors(type, false))
            {
                var @params = constructorInfo.GetParameters();
                if (@params.Length == 9)
                {
                    V1 = AccessTools2.GetDelegate<V1Delegate>(constructorInfo);
                }
            }
        }

        public static InformationMessageWrapper? Create(string information, Color color)
        {
            if (V1 is not null)
            {
                var obj = V1(information, color);
                return InformationMessageWrapper.Create(obj);
            }

            return null;
        }
    }
}