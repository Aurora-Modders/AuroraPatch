using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using AuroraPatch;
using Lib;

namespace ExamplePatch
{
    public class ExamplePatch : Patch
    {
        public override IEnumerable<string> Dependencies { get { return new[] { "Lib" }; } }

        protected override void Load(Assembly aurora)
        {
            Logger.LogInfo("Loading ExamplePatch...");

            // get the exe and its checksum
            var exe = AuroraExecutable;
            var checksum = AuroraChecksum;

            // dependency
            var lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");
            var eco = lib.TypeManager.GetFormType(AuroraFormType.Economics);
            Logger.LogInfo($"Economics type name {eco.Name}");
        }

        protected override void Start(Form map)
        {
            // set background color to black
            map.BackColor = Color.Black;

            // invoke arbitrary code on Aurora's UI thread
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);
        }
    }

}
