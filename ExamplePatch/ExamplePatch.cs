using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using HarmonyLib;

using AuroraPatch;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        protected override void Start()
        {
            Program.logger.LogInfo("Loading ExamplePatch...");

            // Save a reference to the tactical map.
            var map = TypeManager.GetFormInstance(AuroraFormType.TacticalMap);

            // Get the checksum of the exe you're patching.
            var checksum = Program.AuroraChecksum;

            // Get its directory.
            var dir = Program.AuroraExecutableDirectory;

            // Invoke arbitrary code on Aurora's UI thread.
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);

            // Harmony support example.
            // Find and patch the Economics Form's InitializeComponent obfuscated method.
            // As of Aurora 1.13, that method has no parameters, a void return type, and a body size of 627.
            var harmony = new Harmony("some.id");
            var type = TypeManager.GetFormType(AuroraFormType.Economics);
            var methods = TypeManager.GetTypeMethods(type, maxParameters: 0, returnType: typeof(void), minBodySize: 600, maxBodySize: 700);
            var original = AccessTools.Method(type, methods.First().Name);
            var prefix = SymbolExtensions.GetMethodInfo(() => PatchMethod());
            harmony.Patch(original, new HarmonyMethod(prefix));

            // Press the 5 day increment button 10 times in a row.
            IEnumerable<Button> buttons = TypeManager.GetTypeFields<Button, Form>(map);
            Button button5d = buttons.Single(b => b.Name == "cmdIncrement5D");
            for (int i = 0; i < 10; i++)
            {
                action = new Action(() => button5d.PerformClick());
                InvokeOnUIThread(action);
            }

            // Add an event handler to pressing the 30d button.
            Button button30d = buttons.Single(b => b.Name == "cmdIncrement30D");
            action = new Action(() => button30d.Click += Button30D);
            InvokeOnUIThread(action);

            Program.logger.LogInfo("ExamplePatch loaded successfully");

            // Patch.Start is run on its own background thread, no need to return.
            while (true)
            {
                Thread.Sleep(15 * 60 * 1000);

                action = new Action(() => MessageBox.Show("Another 15 minutes have passed"));
                InvokeOnUIThread(action);
            }
        }

        private static void PatchMethod()
        {
            MessageBox.Show("Harmony patched");
        }

        private static void Button30D(object sender, EventArgs e)
        {
            // UI event handlers are always run on the ui thread.
            MessageBox.Show("You've pressed the 30d increment button");
        }
    }

}
