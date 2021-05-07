using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Automation
{
    public class Automation : AuroraPatch.Patch
    {
        public override IEnumerable<string> Dependencies => new[] { "Lib" };

        protected override void Start()
        {
            var lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");

            foreach (var button in lib.KnowledgeBase.GetTimeIncrementButtons())
            {
                lib.LogInfo($"Button {button.Name} click patched");
                button.Click += OnButtonClick;
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            var lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");
            var type = lib.SignatureManager.Get(Lib.AuroraType.EconomicsForm);
            var form = (Form)Activator.CreateInstance(type);

            form.Show();
        }
    }
}
