using Bannerlord.BUTR.Shared.Helpers;

using HarmonyLib.BUTR.Extensions;

using System.IO;
using System.Xml;

using TaleWorlds.Engine;

using Path = System.IO.Path;

namespace Bannerlord.Harmony.Utils
{
    internal static class LocalizedTextManagerUtils
    {
        private delegate XmlDocument? LoadXmlFileDelegate(string path);
        private static readonly LoadXmlFileDelegate? LoadXmlFile =
            AccessTools2.GetDeclaredDelegate<LoadXmlFileDelegate>("TaleWorlds.Localization.LocalizedTextManager:LoadXmlFile");

        private delegate void LoadFromXmlDelegate(XmlDocument doc, string modulePath);
        private static readonly LoadFromXmlDelegate? LoadFromXml =
            AccessTools2.GetDeclaredDelegate<LoadFromXmlDelegate>("TaleWorlds.Localization.LanguageData:LoadFromXml");

        public static void LoadLanguageData()
        {
            if (LoadXmlFile is null || LoadFromXml is null) return;

            var moduleInfo = ModuleInfoHelper.GetModuleByType(typeof(LocalizedTextManagerUtils));
            if (moduleInfo is null) return;

            var path = Path.Combine(Utilities.GetBasePath(), "Modules", moduleInfo.Id, "ModuleData", "Languages");
            if (!Directory.Exists(path)) return;

            foreach (var file in Directory.GetFiles(path, "language_data._xml", SearchOption.AllDirectories))
            {
                if (LoadXmlFile(file) is { } xmlDocument)
                    LoadFromXml(xmlDocument, path);
            }
        }
    }
}