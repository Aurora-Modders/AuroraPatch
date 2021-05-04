using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using HarmonyLib;

using AuroraPatch;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        protected override void Start(Form map)
        {
            Program.Logger.LogInfo("Loading ExamplePatch...");

            // get the checksum of the exe you're patching
            var checksum = Program.AuroraChecksum;

            Program.Logger.LogInfo($"Checksum {checksum}");

            // get its directory
            var dir = Program.AuroraExecutableDirectory;

            // invoke arbitrary code on Aurora's UI thread
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);

            return;

            // Harmony support
            var harmony = new Harmony("some.id");
            var type = map.GetType().Assembly.GetTypes().Single(t => t.Name == "Economics");
            var original = AccessTools.Method(type, "InitializeComponent");
            var prefix = SymbolExtensions.GetMethodInfo(() => PatchMethod());
            harmony.Patch(original, new HarmonyMethod(prefix));
        }

        private static void PatchMethod()
        {
            MessageBox.Show("Harmony patched");
        }
    }

}
