using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

using Path = System.IO.Path;

namespace Bannerlord.Harmony
{
    public class SubModule : MBSubModuleBase
    {
        private const uint COLOR_RED = 0xFF0000;
        private const uint COLOR_ORANGE = 0xFF8000;

        private const string SWarningTitle =
"{=qZXqV8GzUH}Warning from Bannerlord.Harmony!";
        private const string SErrorHarmonyNotFound =
"{=EEVJa5azpB}Bannerlord.Harmony module was not found!";
        private const string SErrorHarmonyNotFirst =
 @"{=NxkNTUUV32}Bannerlord.Harmony is not first in loading order!
This is not recommended. Expect issues!";

        private const string SErrorHarmonyWrongVersion =
 @"{=Z4d2nSD38a}Loaded 0Harmony.dll version is wrong!
Expected {P_VERSION}, but got {E_VERSION}!
This is not recommended. Expect issues!";
        private const string SErrorHarmonyLoadedFromAnotherPlace =
@"{=ASjx7sqkJs}0Harmony.dll was loaded from another location!
This is not recommended. Expect issues!";

        private static readonly HarmonyRef Harmony = new HarmonyRef("Bannerlord.ButterLib.GauntletUISubModule");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            LoadHarmony();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            Harmony.Patch(
                AccessTools.DeclaredMethod(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
                postfix: new HarmonyMethod(typeof(SubModule), nameof(OnBeforeInitialModuleScreenSetAsRootPostfix)));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void OnBeforeInitialModuleScreenSetAsRootPostfix(MBSubModuleBase __instance)
        {
            if (__instance.GetType().Name.Contains("GauntletUISubModule"))
            {
                CheckHarmonyLoadOrder();
                Harmony.Unpatch(
                    AccessTools.DeclaredMethod(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
                    HarmonyPatchType.All,
                    Harmony.Id);
            }
        }

        private static void CheckHarmonyLoadOrder()
        {
            var loadedModules = GetLoadedModules().ToArray();
            var harmonyModule = loadedModules.SingleOrDefault(x => x.Id == "Bannerlord.Harmony");
            var harmonyModuleIndex = harmonyModule is not null ? loadedModules.IndexOf(harmonyModule) : -1;
            if (harmonyModuleIndex == -1)
                InformationManager.DisplayMessage(new InformationMessage(new TextObject(SErrorHarmonyNotFound).ToString(), Color.FromUint(COLOR_RED)));
            if (harmonyModuleIndex != 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(new TextObject(SErrorHarmonyNotFirst).ToString(), Color.FromUint(COLOR_RED)));
            }
        }

        private static void LoadHarmony()
        {
            var binSubFolder = string.IsNullOrWhiteSpace(Common.ConfigName) ? "Win64_Shipping_Client" : Common.ConfigName;
            var providedHarmonyLocation = Path.Combine(Utilities.GetBasePath(), "Modules", "Bannerlord.Harmony", "bin", binSubFolder, "0Harmony.dll");
            var providedHarmony = AssemblyName.GetAssemblyName(providedHarmonyLocation);

            var existingHarmony = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .FirstOrDefault(a => Path.GetFileName(a.Location) == "0Harmony.dll");
            if (existingHarmony != null)
            {
                var existingHarmonyName = existingHarmony.GetName();
                var sb = new StringBuilder();

                if (string.IsNullOrEmpty(existingHarmony.Location) || Path.GetFullPath(providedHarmonyLocation) != Path.GetFullPath(existingHarmony.Location))
                {
                    if (sb.Length != 0) sb.AppendLine();
                    sb.AppendLine(new TextObject(SErrorHarmonyLoadedFromAnotherPlace).ToString());
                }

                if (providedHarmony.Version != existingHarmonyName.Version)
                {
                    if (sb.Length != 0) sb.AppendLine();
                    sb.AppendLine(new TextObject(SErrorHarmonyWrongVersion, new Dictionary<string, TextObject>()
                    {
                        {"P_VERSION", new TextObject(providedHarmony.Version.ToString())},
                        {"E_VERSION", new TextObject(existingHarmonyName.Version.ToString())}
                    }).ToString());
                }

                if (sb.Length > 0)
                {
                    Task.Run(() => MessageBox.Show(sb.ToString(), new TextObject(SWarningTitle).ToString(), MessageBoxButtons.OK));
                    throw new Exception();
                }
            }
            else
            {
                // We shouldn't hit this place.
                AppDomain.CurrentDomain.Load(providedHarmony);
            }
        }

        private static IEnumerable<ModuleInfo> GetLoadedModules()
        {
            var modulesNames = Utilities.GetModulesNames();
            for (var i = 0; i < modulesNames.Length; i++)
            {
                var moduleInfo = new ModuleInfo();
                moduleInfo.Load(modulesNames[i]);
                yield return moduleInfo;
            }
        }
    }
}