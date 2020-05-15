using AuroraPatch;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        private Form TacticalMap { get; set; } = null;

        protected override void Start(Form map)
        {
            // save a reference to the tactical map
            TacticalMap = map;

            // get the checksum of the exe you're patching
            var checksum = Program.AuroraChecksum;

            // get its dirrectory
            var dir = Program.AuroraExecutableDirectory;

            // invoke arbitrary code on Aurora's UI thread
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);

            // Harmony support
            var harmony = new Harmony("some.id");
            var type = map.GetType().Assembly.GetTypes().Single(t => t.Name == "Economics");
            var original = AccessTools.Method(type, "InitializeComponent");
            var prefix = SymbolExtensions.GetMethodInfo(() => PatchMethod());
            harmony.Patch(original, new HarmonyMethod(prefix));

            // Press the 5 day increment button 10 times in a row
            Button button5d = null;

            foreach (var field in map.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType == typeof(Button))
                {
                    var button = field.GetValue(map) as Button;
                    if (button.Name == "cmdIncrement5D")
                    {
                        button5d = button;
                        break;
                    }
                }
            }

            if (button5d == null)
            {
                throw new Exception("button5d not found");
            }

            for (int i = 0; i < 10; i++)
            {
                action = new Action(() => button5d.PerformClick());
                InvokeOnUIThread(action);
            }

            // Add an event handler to pressing the 30d button
            Button button30d = null;

            foreach (var field in map.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType == typeof(Button))
                {
                    var button = field.GetValue(map) as Button;
                    if (button.Name == "cmdIncrement30D")
                    {
                        button30d = button;
                        break;
                    }
                }
            }

            if (button30d == null)
            {
                throw new Exception("button30d not found");
            }

            action = new Action(() => button30d.Click += Button30D);
            InvokeOnUIThread(action);

            // Patch.Start is run on its own background thread, no need to return
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
            // ui event handlers are always run on the ui thread
            MessageBox.Show("You've pressed the 30d increment button");
        }
    }

}
