using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using HarmonyLib;
using AuroraPatch;
using Lib;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        public override IEnumerable<string> Dependencies { get { return new[] { "Lib" }; } }

        protected override void Load(Assembly aurora, Harmony harmony)
        {
            Logger.LogInfo("Loading ExamplePatch...");

            // get the exe and its checksum
            var exe = AuroraExecutable;
            var checksum = AuroraChecksum;

            // dependency
            var lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");
            var map = lib.TypeManager.GetFormType(AuroraFormType.TacticalMap);

            // Harmony
            var ctor = (MethodBase)map.GetMember(".ctor", AccessTools.all)[0];
            var method = new HarmonyMethod(GetType().GetMethod("PatchTacticalMapConstructor", AccessTools.all));
            harmony.Patch(ctor, null, method);
        }

        protected override void Start(Form map)
        {
            // invoke arbitrary code on Aurora's UI thread
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);
        }

        private static void PatchTacticalMapConstructor(Form __instance)
        {
            // set background color to black
            __instance.BackColor = Color.Black;

            MessageBox.Show("Harmony patched!");
        }
    }

}
