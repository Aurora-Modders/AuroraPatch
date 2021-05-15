using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AuroraPatch
{
    public partial class AuroraPatchForm : Form
    {
        private readonly Loader Loader;
        private readonly List<Patch> Patches;

        private void HideNotClose(object sender, FormClosingEventArgs e)
        {
            // If Aurora hasn't been started yet (indicated by the "started" flag in Loader class), closing the window should kill the entire application, not only remove the window.
            if (e.CloseReason == CloseReason.UserClosing && Loader.started)
            {
                e.Cancel = true;
                ((Control)sender).Hide();
            }
            if (e.CloseReason == CloseReason.UserClosing && !Loader.started) Environment.Exit(0);
        }

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
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            foreach (var missing in Loader.GetMissingDependencies(Patches))
            {
                MessageBox.Show($"Patch {missing.Key.Name} missing dependency {missing.Value}");
                
                return;
            }

            try
            {
                Loader.StartAurora(Patches);
            }
            catch (Exception ex)
            {
                Program.Logger.LogCritical($"Failed to start Aurora. {ex}");
                MessageBox.Show("Failed to start Aurora.");
            }
        }

        private void UpdateList()
        {
            ListPatches.Items.Clear();
            ListPatches.Items.AddRange(Patches.Select(p => p.Name).ToArray());

            if (ListPatches.Items.Count > 0)
            {
                ListPatches.SelectedIndex = 0;
            }

            UpdateDescription();
        }

        private void UpdateDescription()
        {
            var index = ListPatches.SelectedIndex;
            if (index >= 0)
            {
                var patch = Patches.Single(p => p.Name == (string)ListPatches.Items[index]);
                LabelDescription.Text = $"Description: {patch.Description}";
            }
            else
            {
                LabelDescription.Text = "Description:";
            }     
        }

        private void ButtonChangeSettings_Click(object sender, EventArgs e)
        {
            var index = ListPatches.SelectedIndex;
            if (index >= 0)
            {
                var patch = Patches.Single(p => p.Name == (string)ListPatches.Items[index]);
                try
                {
                    patch.ChangeSettingsInternal();
                }
                catch (Exception ex)
                {
                    Program.Logger.LogError($"Failed to change settings for Patch {patch.Name}. {ex}");
                }
            }
        }

        private void ListPatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDescription();
        }
    }
}
