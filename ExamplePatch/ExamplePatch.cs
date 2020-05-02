using AuroraPatch;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        internal static Form TacticalMap { get; private set; } = null;

        protected override void Start(Form map)
        {
            TacticalMap = map;

            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);

            var harmony = new Harmony("some.id");
            var method = (MethodBase)map.GetType().Assembly.GetTypes()
                .Single(type => type.Name == "Economics")
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "InitializeComponent");

            harmony.Patch(method, new HarmonyMethod(((Action)PatchMethod).Method));
        }

        private static void PatchMethod()
        {
            MessageBox.Show("Harmony patched");
        }
    }

}
