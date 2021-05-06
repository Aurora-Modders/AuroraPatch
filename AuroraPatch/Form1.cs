using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AuroraPatch
{
    public partial class Form1 : Form
    {
        private readonly Loader Loader;
        private readonly List<Patch> Patches;

        internal Form1(Loader loader) : base()
        {
            InitializeComponent();

            Loader = loader;
            Patches = Loader.FindPatches();
            Loader.SortPatches(Patches);

            UpdateList();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            foreach (var missing in Loader.GetUnmetDependencies(Patches))
            {
                MessageBox.Show($"Patch {missing.Key.Name} missing dependency {missing.Value}");
                
                return;
            }

            Loader.StartAurora(Patches);
        }

        private void UpdateList()
        {
            ListPatches.Items.Clear();
            ListPatches.Items.AddRange(Patches.Select(p => p.Name).ToArray());
        }
    }
}
