using Bannerlord.BUTR.Shared.Helpers;
using Bannerlord.Harmony.Utils;

using HarmonyLib;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TaleWorlds.Engine;
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
        private const string SErrorHarmonyLibNotFound = @"{=HSyaj6TjUG}0Harmony.dll file was not found!";

        private const string SErrorHarmonyWrongVersion = @"{=Z4d2nSD38a}Loaded 0Harmony.dll version is wrong!{NL}Expected {P_VERSION}, but got {E_VERSION}!{EXPECT_ISSUES_WARNING}";
        private const string SErrorHarmonyLoadedFromAnotherPlace = @"{=ASjx7sqkJs}0Harmony.dll was loaded from another location: {LOCATION}!{NL}It may be caused by a custom launcher or some other mod!{EXPECT_ISSUES_WARNING}";

        private const string SWarningExpectIssues = @"{=xTeLdSrXk4}{NL}This is not recommended. Expect issues!{NL}If your game crashes and you had this warning, please, mention it in the bug report!";

        private static readonly HarmonyRef Harmony = new("Bannerlord.Harmony.GauntletUISubModule");

        private readonly DebugUI _debugUI = new();

        private static TextObject? GetExpectIssuesWarning() =>
            TextObjectHelper.Create(SWarningExpectIssues)?.SetTextVariable2("NL", Environment.NewLine);

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            LoadHarmony();

            if (ApplicationVersionHelper.GameVersion() is { } gameVersion)
            {
                if (gameVersion.Major is 1 && gameVersion.Minor is 8 && gameVersion.Revision is >= 0)
                {
                    LocalizedTextManagerUtils.LoadLanguageData();
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            Harmony.Patch(
                AccessTools.Method(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
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
                CheckLoadOrder();
                Harmony.Unpatch(
                    AccessTools.Method(typeof(MBSubModuleBase), "OnBeforeInitialModuleScreenSetAsRoot"),
                    HarmonyPatchType.All,
                    Harmony.Id);
            }
        }

        private static void CheckLoadOrder()
        {
            var loadedModules = ModuleInfoHelper.GetLoadedModules().ToList();
            var harmonyModule = loadedModules.SingleOrDefault(x => x.Id == "Bannerlord.Harmony");
            var harmonyModuleIndex = harmonyModule is not null ? loadedModules.IndexOf(harmonyModule) : -1;
            if (harmonyModuleIndex == -1)
                InformationManagerUtils.DisplayMessage(InformationMessageUtils.Create(TextObjectHelper.Create(SErrorHarmonyNotFound)?.ToString() ?? "ERROR", Color.FromUint(COLOR_RED)));
            if (harmonyModuleIndex != 0)
            {
                var textObject = TextObjectHelper.Create(SErrorHarmonyNotFirst)?.SetTextVariable2("EXPECT_ISSUES_WARNING", GetExpectIssuesWarning());
                InformationManagerUtils.DisplayMessage(InformationMessageUtils.Create(textObject?.ToString() ?? "ERROR", Color.FromUint(COLOR_RED)));
            }
        }

        private static void LoadHarmony()
        {
            var binSubFolder = string.IsNullOrWhiteSpace(Common.ConfigName) ? "Win64_Shipping_Client" : Common.ConfigName;
            var providedHarmonyLocation = Path.Combine(Utilities.GetBasePath(), "Modules", "Bannerlord.Harmony", "bin", binSubFolder, "0Harmony.dll");

            if (!File.Exists(providedHarmonyLocation))
            {
                Task.Run(() => MessageBox.Show(TextObjectHelper.Create(SErrorHarmonyLibNotFound)?.ToString() ?? "ERROR",
                    TextObjectHelper.Create(SWarningTitle)?.ToString() ?? "ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, (MessageBoxOptions) 0x40000));
                return;
            }

            var providedHarmony = AssemblyName.GetAssemblyName(providedHarmonyLocation);

            var existingHarmony = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .FirstOrDefault(a => Path.GetFileName(a.Location) == "0Harmony.dll");
            if (existingHarmony is not null)
            {
                var existingHarmonyName = existingHarmony.GetName();
                var sb = new StringBuilder();

                if (string.IsNullOrEmpty(existingHarmony.Location) || Path.GetFullPath(providedHarmonyLocation) != Path.GetFullPath(existingHarmony.Location))
                {
                    if (sb.Length != 0) sb.AppendLine();
                    var textObject = TextObjectHelper.Create(SErrorHarmonyLoadedFromAnotherPlace);
                    textObject?.SetTextVariable2("LOCATION", TextObjectHelper.Create(string.IsNullOrEmpty(existingHarmony.Location) ? string.Empty : Path.GetFullPath(existingHarmony.Location)));
                    textObject?.SetTextVariable2("EXPECT_ISSUES_WARNING", GetExpectIssuesWarning());
                    textObject?.SetTextVariable2("NL", Environment.NewLine);
                    sb.AppendLine(textObject?.ToString() ?? "ERROR");
                }

                if (providedHarmony.Version != existingHarmonyName.Version)
                {
                    if (sb.Length != 0) sb.AppendLine();
                    var textObject = TextObjectHelper.Create(SErrorHarmonyWrongVersion);
                    textObject?.SetTextVariable2("P_VERSION", TextObjectHelper.Create(providedHarmony.Version.ToString()));
                    textObject?.SetTextVariable2("E_VERSION", TextObjectHelper.Create(existingHarmonyName.Version.ToString()));
                    textObject?.SetTextVariable2("EXPECT_ISSUES_WARNING", GetExpectIssuesWarning());
                    textObject?.SetTextVariable2("NL", Environment.NewLine);
                    sb.AppendLine(textObject?.ToString() ?? "ERROR");
                }

                if (sb.Length > 0)
                {
                    Task.Run(() => MessageBox.Show(sb.ToString(),
                        TextObjectHelper.Create(SWarningTitle)?.ToString() ?? "ERROR", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, (MessageBoxOptions) 0x40000));
                }
            }
            else
            {
                // We shouldn't hit this place.
                AppDomain.CurrentDomain.Load(providedHarmony);
            }
        }
    }
}