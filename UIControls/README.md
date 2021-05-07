# UIControls

Patch primarily used by other patch authors to modify Aurora form controls when shown.

## Usage Example

    using System.Drawing;
    using System.Collections.Generic;
    using HarmonyLib;

    namespace PatchName
    {
        public class PatchName : AuroraPatch.Patch
        {
			public override IEnumerable<string> Dependencies => new[] { "UIControls" };
			protected override void Load(Harmony harmony)
			{
				// Swap all blue to black.
				GlobalColorSwaps.Add(Color.FromArgb(0, 0, 64), Color.Black);
				// Change all button colors to Red.
				TypeCallbacks[typeof(Button)] = new List<Action<Control>>
				{
					ctrl => ctrl.BackColor = Color.Red
				};
				// Change the 30 day increment button to Blue.
				ControlCallbacks.Add(ctrl => {
					if (ctrl.Name == "cmdIncrement30D")
					{
						ctrl.BackColor = Color.Blue;
					}
				});
			}
		}
	}
