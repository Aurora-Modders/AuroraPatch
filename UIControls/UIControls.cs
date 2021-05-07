using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using HarmonyLib;

namespace UIControls
{
    /// <summary>
    /// Aurora patch that deals with the modification of any/all Form Control components.
    /// TODO: Currently only triggers actions on Form.Shown event - investigate other trigger options.
    /// </summary>
    public class UIControls : AuroraPatch.Patch
    {
        public override string Description => "API to access and tweak various UI control components.";

        // Global Color Swaps.
        public static Dictionary<Color, Color> GlobalColorSwaps = new Dictionary<Color, Color>();
        // Type-Filtered Callbacks
        // TODO: Add helper method so user doesn't need to check if Type list already exists before adding to it.
        public static Dictionary<Type, List<Action<Control>>> TypeCallbacks = new Dictionary<Type, List<Action<Control>>>();
        // Generic Control Callbacks.
        public static List<Action<Control>> ControlCallbacks = new List<Action<Control>>();

        /// <summary>
        /// Load our patch and setup our custom Shown event handler on every form.
        /// </summary>
        /// <param name="harmony"></param>
        protected override void Load(Harmony harmony)
        {
            HarmonyMethod postfixMethod = new HarmonyMethod(GetType().GetMethod("FormConstructorPostfix", AccessTools.all));
            foreach (var type in AuroraAssembly.GetTypes())
            {
                if (typeof(Form).IsAssignableFrom(type))
                {
                    foreach (var ctor in type.GetConstructors())
                    {
                        harmony.Patch(ctor, postfix: postfixMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Our harmony form constructor postfix method which simply adds our custom shown callback.
        /// </summary>
        /// <param name="__instance"></param>
        private static void FormConstructorPostfix(Form __instance)
        {
            __instance.Shown += CustomShown;
        }

        /// <summary>
        /// Our custom shown callback is a pass-through method that simply casts our generic callback
        /// object to a Control and sends it off to the IterateControls method for enumeration/modification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CustomShown(Object sender, EventArgs e)
        {
            IterateControls((Control)sender);
        }

        /// <summary>
        /// Recusively loops through all controls and applies our requested changes.
        /// </summary>
        /// <param name="control"></param>
        private static void IterateControls(Control control)
        {
            ApplyChanges(control);
            foreach (Control ctrl in control.Controls)
            {
                IterateControls(ctrl);
            }
        }

        /// <summary>
        /// The meat of our patch. Does all the heavy lifting to expose our API to Aurora controls.
        /// </summary>
        /// <param name="control"></param>
        private static void ApplyChanges(Control control)
        {
            // Global Color Swaps.
            if (GlobalColorSwaps.ContainsKey(control.BackColor)) control.BackColor = GlobalColorSwaps[control.BackColor];
            if (GlobalColorSwaps.ContainsKey(control.ForeColor)) control.ForeColor = GlobalColorSwaps[control.ForeColor];
            // Type-Filtered Callbacks.
            if (TypeCallbacks.ContainsKey(control.GetType()))
            {
                foreach (Action<Control> callback in TypeCallbacks[control.GetType()])
                {
                    callback.Invoke(control);
                }
            }
            // Generic Control Callbacks.
            foreach (Action<Control> callback in ControlCallbacks)
            {
                callback.Invoke(control);
            }
        }
    }
}
