using System;
using System.Linq;

using Tact.Reflection;

using TaleWorlds.Localization;

namespace Bannerlord.BUTRLoader.Helpers
{
    internal static class TextObjectUtils
    {
        private static readonly Func<object[], object>? TextObjectInvoker;

        static TextObjectUtils()
        {
            if (typeof(TextObject).GetConstructors().FirstOrDefault() is { } constructorInfo)
                TextObjectInvoker = EfficientInvoker.ForConstructor(constructorInfo);
        }

        public static TextObject? Create(string value) => TextObjectInvoker?.Invoke(new object[] { value, null! }) as TextObject;
    }
}