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

            // invoke arbitrary code on Aurora's UI thread
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);

            // Harmony support
            var harmony = new Harmony("some.id");
            var method = (MethodBase)map.GetType().Assembly.GetTypes()
                .Single(type => type.Name == "Economics")
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "InitializeComponent");

            harmony.Patch(method, new HarmonyMethod(((Action)PatchMethod).Method));

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
    }

}
