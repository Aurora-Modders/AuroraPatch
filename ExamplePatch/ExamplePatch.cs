using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using HarmonyLib;
using AuroraPatch;
using Lib;
using System.Data;

namespace ExamplePatch
{
    public class ExamplePatch : AuroraPatch.Patch
    {
        public static Color BackColor { get; set; } = Color.Black;

        public override string Description => "An example patch.";
        public override IEnumerable<string> Dependencies => new[] { "Lib" };

        protected override void Load(Harmony harmony)
        {
            Logger.LogInfo("Loading ExamplePatch...");

            try
            {
                BackColor = Deserialize<Color>("color");
            }
            catch (Exception e)
            {
                Logger.LogInfo("saved color not found");
            }

            // get the exe and its checksum
            var exe = AuroraExecutable;
            var checksum = AuroraChecksum;

            // dependency
            var lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");
            var map = lib.SignatureManager.Get(AuroraType.TacticalMapForm);

            // Harmony
            var ctor = (MethodBase)map.GetMember(".ctor", AccessTools.all)[0];
            var method = new HarmonyMethod(GetType().GetMethod("PatchTacticalMapConstructor", AccessTools.all));
            harmony.Patch(ctor, null, method);
        }

        protected override void Start()
        {
            // read in-memory db
            var lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");
            var table = lib.DatabaseManager.ExecuteQuery("SELECT RaceName FROM FCT_Race");

            // invoke arbitrary code on Aurora's UI thread
            var message = "Example patch loaded!\n";
            foreach (DataRow row in table.Rows)
            {
                message += $"Race name: {row[0]}\n";
            }

            var action = new Action(() => MessageBox.Show(message));
            InvokeOnUIThread(action);
        }

        protected override void ChangeSettings()
        {
            base.ChangeSettings();

            // pick TacticalMap background color

            var diag = new ColorDialog();
            diag.Color = BackColor;
            
            var result = diag.ShowDialog();
            if (result == DialogResult.OK)
            {
                BackColor = diag.Color;

                if (TacticalMap != null)
                {
                    var action = new Action(() => TacticalMap.BackColor = BackColor);
                    InvokeOnUIThread(action);
                }

                Serialize("color", BackColor);
            }
        }

        private static void PatchTacticalMapConstructor(Form __instance)
        {
            // set background color to black
            __instance.BackColor = BackColor;

            MessageBox.Show("Harmony patched!");
        }
    }

}
