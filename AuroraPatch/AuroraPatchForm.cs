using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AuroraPatch
{
    public partial class AuroraPatchForm : Form
    {
        private readonly Loader      Loader;
        private readonly List<Patch> Patches;
        private          bool        IgnoreCheck;

        internal AuroraPatchForm(Loader loader) : base()
        {
            InitializeComponent();

            Loader = loader;
            try
            {
                Patches = Loader.FindPatches();
                Loader.SortPatches(Patches);
            }
            catch (Exception e)
            {
                Program.Logger.LogCritical($"Failed to find patches. {e}");
                MessageBox.Show("Failed to find patches. You can still play Aurora without patches.");
            }

            UpdateList();
            var selected = loader.LoadSelectedPatches();
            for (int i = 0; i < CheckedListPatches.Items.Count; i++)
            {

                var thing = CheckedListPatches.Items[i];
                var strname = thing.ToString();
                if(selected.Contains(strname))
                    CheckedListPatches.SetItemChecked(i, true);
                
            }
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            foreach (var missing in Loader.GetMissingDependencies(Patches))
            {
                MessageBox.Show($"Patch {missing.Key.Name} missing dependency {missing.Value}");

                return;
            }

            ButtonStart.Enabled = false;
            Refresh();

            try
            {
                List<Patch> selectedPatches = new List<Patch>();

                if (CheckedListPatches.CheckedItems.Count != 0)
                {
                    selectedPatches.AddRange(from object checkedItem in CheckedListPatches.CheckedItems
                                             select Patches.Find(patch => patch.Name == checkedItem.ToString()));
                }

                Loader.StartAurora(selectedPatches);
                Loader.SaveSelectedPatches(selectedPatches);
            }
            catch (Exception ex)
            {
                Program.Logger.LogCritical($"Failed to start Aurora. {ex}");
                MessageBox.Show("Failed to start Aurora.");
            }
        }

        private void UpdateList()
        {
            // Co-variant array conversion from string[] to object[] can cause run-time exception on write operation
            CheckedListPatches.Items.AddRange(Patches.Select(patch => patch.Name).ToArray());

            if (CheckedListPatches.Items.Count > 0)
            {
                CheckedListPatches.SelectedIndex = 0;
            }

            UpdateDescription();
            UpdateSettings();
        }

        private void UpdateDescription()
        {
            int index = CheckedListPatches.SelectedIndex;
            if (index >= 0)
            {
                var patch = Patches.Single(p => p.Name == (string)CheckedListPatches.Items[index]);
                LabelDescription.Text = $"Description: {patch.Description}";
            }
            else
            {
                LabelDescription.Text = "No patch selected...";
            }
        }

        private void UpdateSettings()
        {
            int index = CheckedListPatches.SelectedIndex;
            if (index >= 0)
            {
                var                patch = Patches.Single(p => p.Name == (string)CheckedListPatches.Items[index]);
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;
                bool               hasSettings = patch.GetType().GetMethod("ChangeSettings", flags) != null;

                ButtonChangeSettings.Enabled = hasSettings;
            }
            else
            {
                ButtonChangeSettings.Enabled = false;
            }
        }

        private void ButtonChangeSettings_Click(object sender, EventArgs e)
        {
            int index = CheckedListPatches.SelectedIndex;
            if (index < 0) return;

            var patch = Patches.Single(p => p.Name == (string)CheckedListPatches.Items[index]);
            try
            {
                patch.ChangeSettingsInternal();
            }
            catch (Exception ex)
            {
                Program.Logger.LogError($"Failed to change settings for Patch {patch.Name}. {ex}");
            }
        }

        private void AuroraPatchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Loader.TacticalMap == null || Loader.TacticalMap.Visible == false)
            {
                Application.Exit();
            }
        }

        private void CheckedListPatches_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (IgnoreCheck) e.NewValue = e.CurrentValue;
        }

        private void CheckedListPatches_MouseClick(object sender, MouseEventArgs e)
        {
            IgnoreCheck = e.X > SystemInformation.MenuCheckSize.Width;
        }

        private void CheckedListPatches_MouseUp(object sender, MouseEventArgs e)
        {
            IgnoreCheck = false;
        }

        private void CheckedListPatches_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDescription();
            UpdateSettings();
        }
    }
}