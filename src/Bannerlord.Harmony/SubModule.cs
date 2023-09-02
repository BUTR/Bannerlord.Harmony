using Bannerlord.BUTR.Shared.Helpers;

using HarmonyLib;
using HarmonyLib.BUTR.Extensions;

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TaleWorlds.InputSystem;
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

        // We can't rely on EN since the game assumes that the default locale is always English
        private const string SWarningTitle = @"{=qZXqV8GzUH}Warning from Bannerlord.Harmony!";
        private const string SErrorHarmonyNotFound = @"{=EEVJa5azpB}Bannerlord.Harmony module was not found!";
        private const string SErrorHarmonyNotFirst = @"{=NxkNTUUV32}Bannerlord.Harmony is not first in loading order!{EXPECT_ISSUES_WARNING}";

        private const string SErrorHarmonyWrongVersion = @"{=Z4d2nSD38a}Loaded 0Harmony.dll version is wrong!{NL}Expected {P_VERSION}, but got {E_VERSION}!{EXPECT_ISSUES_WARNING}";
        private const string SErrorHarmonyLoadedFromAnotherPlace = @"{=ASjx7sqkJs}0Harmony.dll was loaded from another location: {LOCATION}!{NL}It may be caused by a custom launcher or some other mod!{EXPECT_ISSUES_WARNING}";

        private const string SWarningExpectIssues = @"{=xTeLdSrXk4}{NL}This is not recommended. Expect issues!{NL}If your game crashes and you had this warning, please, mention it in the bug report!";

        private static readonly HarmonyRef Harmony = new("Bannerlord.Harmony.GauntletUISubModule");
        private static readonly HarmonyRef Harmony2 = new("Bannerlord.Harmony.UnpatchAll");

        private readonly DebugUI _debugUI = new();

        private static TextObject? GetExpectIssuesWarning() => new TextObject(SWarningExpectIssues)?.SetTextVariable("NL", Environment.NewLine);

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            ValidateHarmony();

            Harmony2.Patch(
                SymbolExtensions2.GetMethodInfo((HarmonyLib.Harmony x) => x.UnpatchAll(null)),
                prefix: new HarmonyMethod(typeof(SubModule), nameof(UnpatchAllPrefix)));
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            Harmony.Patch(
                AccessTools2.Method(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
                postfix: new HarmonyMethod(typeof(SubModule), nameof(OnBeforeInitialModuleScreenSetAsRootPostfix)));
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            if ((Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl)) &&
                (Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt)) &&
                Input.IsKeyPressed(InputKey.H))
            {
                _debugUI.Visible = !_debugUI.Visible;
            }

            _debugUI.Update(dt);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void OnBeforeInitialModuleScreenSetAsRootPostfix(MBSubModuleBase __instance)
        {
            if (__instance.GetType().Name.Contains("GauntletUISubModule"))
            {
                // OnBeforeInitialModuleScreenSetAsRoot will be called before the Native modules
                // will be able to initialize the chat system we use to log info.
                ValidateLoadOrder();
                Harmony.Unpatch(
                    AccessTools2.Method(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
                    HarmonyPatchType.All,
                    Harmony.Id);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UnpatchAllPrefix(string? harmonyID) => harmonyID is not null;

        private static void ValidateLoadOrder()
        {
            var loadedModules = ModuleInfoHelper.GetLoadedModules().ToList();
            var harmonyModule = loadedModules.SingleOrDefault(x => x.Id == "Bannerlord.Harmony");
            var harmonyModuleIndex = harmonyModule is not null ? loadedModules.IndexOf(harmonyModule) : -1;
            if (harmonyModuleIndex == -1)
                InformationManager.DisplayMessage(new InformationMessage(new TextObject(SErrorHarmonyNotFound)?.ToString() ?? "ERROR", Color.FromUint(COLOR_RED)));
            if (harmonyModuleIndex != 0)
            {
                var textObject = new TextObject(SErrorHarmonyNotFirst)?.SetTextVariable("EXPECT_ISSUES_WARNING", GetExpectIssuesWarning());
                InformationManager.DisplayMessage(new InformationMessage(textObject?.ToString() ?? "ERROR", Color.FromUint(COLOR_RED)));
            }
        }

        private static void ValidateHarmony()
        {
            var harmonyType = typeof(HarmonyMethod);

            var currentExistingHarmony = harmonyType.Assembly;
            var currentHarmonyVersion = currentExistingHarmony.GetName().Version ?? new Version(0, 0);
            var requiredHarmonyVersion = typeof(SubModule).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key == "HarmonyVersion") is { } attr
                ? Version.TryParse(attr.Value, out  var v)
                    ? v
                    : new Version(0, 0)
                : new Version(0, 0);

            var sb = new StringBuilder();
            var harmonyModule = ModuleInfoHelper.GetModuleByType(harmonyType);
            if (harmonyModule is null)
            {
                if (sb.Length != 0) sb.AppendLine();
                var textObject = new TextObject(SErrorHarmonyLoadedFromAnotherPlace);
                textObject.SetTextVariable("LOCATION", new TextObject(string.IsNullOrEmpty(currentExistingHarmony.Location) ? string.Empty : Path.GetFullPath(currentExistingHarmony.Location)));
                textObject.SetTextVariable("EXPECT_ISSUES_WARNING", GetExpectIssuesWarning());
                textObject.SetTextVariable("NL", Environment.NewLine);
                sb.AppendLine(textObject.ToString() ?? "ERROR");
            }

            if (requiredHarmonyVersion.CompareTo(currentHarmonyVersion) != 0)
            {
                if (sb.Length != 0) sb.AppendLine();
                var textObject = new TextObject(SErrorHarmonyWrongVersion);
                textObject.SetTextVariable("P_VERSION", new TextObject(requiredHarmonyVersion.ToString()));
                textObject.SetTextVariable("E_VERSION", new TextObject(currentHarmonyVersion.ToString()));
                textObject.SetTextVariable("EXPECT_ISSUES_WARNING", GetExpectIssuesWarning());
                textObject.SetTextVariable("NL", Environment.NewLine);
                sb.AppendLine(textObject.ToString() ?? "ERROR");
            }

            if (sb.Length > 0)
            {
                Task.Run(() => MessageBox.Show(sb.ToString(),
                    new TextObject(SWarningTitle).ToString() ?? "ERROR", System.Windows.Forms.MessageBoxButtons.OK));
            }
        }
    }
}