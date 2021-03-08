using Bannerlord.BUTR.Shared.Extensions;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.Engine;

namespace Bannerlord.Harmony
{
    public class DebugUI
    {
        private readonly string _windowTitle = "Harmony Debug UI";

        public bool Visible { get; set; }

        private float _timeCounter;
        private readonly Dictionary<MethodBase, Patches> _patches = new();

        public void Update(float dt)
        {
            if (Visible)
            {
                Begin();
                AddButtons();
                DisplayPatchedMethods();
                End();

                _timeCounter += dt;
                if (_timeCounter >= 3F)
                {
                    _timeCounter = 0F;

                    _patches.Clear();
                    foreach (var originalMethod in HarmonyLib.Harmony.GetAllPatchedMethods())
                    {
                        var patches = HarmonyLib.Harmony.GetPatchInfo(originalMethod);
                        if (originalMethod is null || patches is null) continue;
                        _patches.Add(originalMethod, patches);
                    }
                }
            }
        }

        private void DisplayPatchedMethods()
        {
            if (!Imgui.TreeNode("Patched Methods"))
            {
                return;
            }

            DisplayPatches();

            Imgui.TreePop();
        }

        private void DisplayPatches()
        {
            foreach (var (originalMethod, patches) in _patches)
            {
                var allPatches = patches.Prefixes.Concat(patches.Postfixes).Concat(patches.Transpilers).Concat(patches.Finalizers);

                if (!allPatches.Any() || !Imgui.TreeNode(originalMethod.FullDescription())) continue;

                Imgui.Columns(3);
                Imgui.Text("Type");
                foreach (var _ in patches.Prefixes)    Imgui.Text("Prefix");
                foreach (var _ in patches.Postfixes)   Imgui.Text("Postfix");
                foreach (var _ in patches.Transpilers) Imgui.Text("Transpiler");
                foreach (var _ in patches.Finalizers)  Imgui.Text("Finalizer");

                Imgui.NextColumn();
                Imgui.Text("Owner");
                foreach (var patch in allPatches)
                {
                    Imgui.Text(patch.owner);
                }

                Imgui.NextColumn();
                Imgui.Text("Method");
                Imgui.Separator();
                foreach (var patch in allPatches)
                {
                    Imgui.Text($"{patch.PatchMethod.DeclaringType!.FullName}.{patch.PatchMethod.Name}");
                }

                Imgui.Columns();

                Imgui.TreePop();
            }
        }

        private void Begin()
        {
            Imgui.BeginMainThreadScope();
            Imgui.Begin(_windowTitle);
        }

        private void AddButtons()
        {
            Imgui.NewLine();

            Imgui.SameLine(20);
            if (Imgui.SmallButton("Close DebugUI"))
            {
                Visible = false;
            }
        }

        private void End()
        {
            Imgui.End();
            Imgui.EndMainThreadScope();
        }
    }
}