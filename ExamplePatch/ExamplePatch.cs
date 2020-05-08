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
