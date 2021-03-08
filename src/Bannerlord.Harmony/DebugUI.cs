using Bannerlord.BUTR.Shared.Extensions;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.Engine;

namespace Bannerlord.Harmony
{
    // Original idea taken from BannerlordCoop
    // https://github.com/Bannerlord-Coop-Team/BannerlordCoop/blob/95606e899763b71f62ed5a0fcfcb2e2d8c1b7e92/source/Coop/Mod/DebugUtil/DebugUI.cs
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

                Imgui.Columns(6);
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
                Imgui.Text("Index");
                foreach (var patch in allPatches)
                {
                    Imgui.Text($"{patch.index}");
                }

                Imgui.NextColumn();
                Imgui.Text("Priority");
                foreach (var patch in allPatches)
                {
                    Imgui.Text($"{patch.priority}");
                }

                Imgui.NextColumn();
                Imgui.Text("Before");
                foreach (var patch in allPatches)
                {
                    Imgui.Text($"{string.Join(",", patch.before)}");
                }

                Imgui.NextColumn();
                Imgui.Text("After");
                Imgui.Separator();
                foreach (var patch in allPatches)
                {
                    Imgui.Text($"{string.Join(",", patch.after)}");
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