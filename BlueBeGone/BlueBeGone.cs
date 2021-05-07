using System.Drawing;
using System.Collections.Generic;

using HarmonyLib;

namespace BlueBeGone
{
    public class BlueBeGone : AuroraPatch.Patch
    {
        public override string Description => "Simplest possible theme that swaps all blue with black.";
        public override IEnumerable<string> Dependencies => new[] { "UIControls" };

        protected override void Load(Harmony harmony)
        {
            UIControls.UIControls.GlobalColorSwaps.Add(Color.FromArgb(0, 0, 64), Color.Black);
        }
    }
}
