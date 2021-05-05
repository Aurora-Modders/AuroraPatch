using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using HarmonyLib;

using AuroraPatch;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        protected override void Load(Assembly aurora)
        {
            Logger.LogInfo("Loading ExamplePatch...");

            // get the exe and its checksum
            var exe = AuroraExecutable;
            var checksum = AuroraChecksum;

            return;

            // Harmony support
            var harmony = new Harmony("some.id");
            var type = aurora.GetTypes().Single(t => t.Name == "Economics");
            var original = AccessTools.Method(type, "InitializeComponent");
            var prefix = SymbolExtensions.GetMethodInfo(() => PatchMethod());
            harmony.Patch(original, new HarmonyMethod(prefix));
        }

        protected override void Start(Form map)
        {
            // set background color to black
            map.BackColor = Color.Black;

            // invoke arbitrary code on Aurora's UI thread
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);
        }

        private static void PatchMethod()
        {
            MessageBox.Show("Harmony patched");
        }
    }

}
