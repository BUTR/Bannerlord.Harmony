using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using System.Linq;
using System.Reflection;

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
            foreach (var constructorInfo in AccessTools.GetDeclaredConstructors(type, false) ?? Enumerable.Empty<ConstructorInfo>())
            {
                var @params = constructorInfo.GetParameters();
                if (@params.Length == 2 && @params[0].ParameterType == typeof(string) && @params[1].ParameterType == typeof(Color))
                {
                    V1 = AccessTools2.GetDelegate<V1Delegate>(constructorInfo);
                }
            }
        }

        public static InformationMessageWrapper? Create(string information, Color color)
        {
            if (V1 is not null)
            {
                return InformationMessageWrapper.Create(V1(information, color));
            }

            return null;
        }
    }
}