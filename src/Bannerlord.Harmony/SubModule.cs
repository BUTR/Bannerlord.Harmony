using Bannerlord.BUTR.Shared.Helpers;
using Bannerlord.BUTR.Shared.ModuleInfoExtended;

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
using TaleWorlds.MountAndBlade;

using Path = System.IO.Path;

namespace Bannerlord.Harmony
{
    public class SubModule : MBSubModuleBase
    {
        private const uint COLOR_RED = 0xFF0000;
        private const uint COLOR_ORANGE = 0xFF8000;

        private const string SWarningTitle =
@"{=qZXqV8GzUH}Warning from Bannerlord.Harmony!";
        private const string SErrorHarmonyNotFound =
@"{=EEVJa5azpB}Bannerlord.Harmony module was not found!";
        private const string SErrorHarmonyNotFirst =
 @"{=NxkNTUUV32}Bannerlord.Harmony is not first in loading order!
This is not recommended. Expect issues!";

        private const string SErrorHarmonyWrongVersion =
 @"{=Z4d2nSD38a}Loaded 0Harmony.dll version is wrong!
Expected {P_VERSION}, but got {E_VERSION}!
This is not recommended. Expect issues!";
        private const string SErrorHarmonyLoadedFromAnotherPlace =
@"{=ASjx7sqkJs}0Harmony.dll was loaded from another location!
It may be caused by a custom launcher!
This is not recommended. Expect issues!";

        private static readonly HarmonyRef Harmony = new("Bannerlord.ButterLib.GauntletUISubModule");

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
                // OnBeforeInitialModuleScreenSetAsRoot will be called before the Native modules
                // will be able to initialize the chat system we use to log info.
                CheckLoadOrder();
                Harmony.Unpatch(
                    AccessTools.DeclaredMethod(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
                    HarmonyPatchType.All,
                    Harmony.Id);
            }
        }

        private static void CheckLoadOrder()
        {
            var loadedModules = GetLoadedModules().ToList();
            var harmonyModule = loadedModules.SingleOrDefault(x => x.Id == "Bannerlord.Harmony");
            var harmonyModuleIndex = harmonyModule is not null ? loadedModules.IndexOf(harmonyModule) : -1;
            if (harmonyModuleIndex == -1)
                InformationManager.DisplayMessage(new InformationMessage(TextObjectUtils.Create(SErrorHarmonyNotFound)?.ToString() ?? "ERROR", Color.FromUint(COLOR_RED)));
            if (harmonyModuleIndex != 0)
                InformationManager.DisplayMessage(new InformationMessage(TextObjectUtils.Create(SErrorHarmonyNotFirst)?.ToString() ?? "ERROR", Color.FromUint(COLOR_RED)));
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
                    sb.AppendLine(TextObjectUtils.Create(SErrorHarmonyLoadedFromAnotherPlace)?.ToString() ?? "ERROR");
                }

                if (providedHarmony.Version != existingHarmonyName.Version)
                {
                    if (sb.Length != 0) sb.AppendLine();
                    var textObject = TextObjectUtils.Create(SErrorHarmonyWrongVersion);
                    textObject?.SetTextVariable("P_VERSION", TextObjectUtils.Create(providedHarmony.Version.ToString()));
                    textObject?.SetTextVariable("E_VERSION", TextObjectUtils.Create(existingHarmonyName.Version.ToString()));
                    sb.AppendLine(textObject?.ToString() ?? "ERROR");
                }

                if (sb.Length > 0)
                {
                    Task.Run(() => MessageBox.Show(sb.ToString(), TextObjectUtils.Create(SWarningTitle)?.ToString() ?? "ERROR", MessageBoxButtons.OK));
                }
            }
            else
            {
                // We shouldn't hit this place.
                AppDomain.CurrentDomain.Load(providedHarmony);
            }
        }

        private static IEnumerable<ModuleInfo2> GetLoadedModules()
        {
            var modulesNames = Utilities.GetModulesNames();
            for (var i = 0; i < modulesNames.Length; i++)
            {
                var moduleInfo = new ModuleInfo2();
                moduleInfo.Load(modulesNames[i]);
                yield return moduleInfo;
            }
        }
    }
}