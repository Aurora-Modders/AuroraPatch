using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuroraPatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            var patches = Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(Program.AuroraExecutable), "Patches"), "*.dll").ToList();
            Program.StartAurora(patches);
        }
    }
}
