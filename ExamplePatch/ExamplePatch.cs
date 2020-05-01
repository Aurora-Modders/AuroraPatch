using AuroraPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExamplePatch
{
    public class ExamplePatch : Patch
    {
        protected override void Start(Form map)
        {
            var action = new Action(() => MessageBox.Show("Example patch loaded!"));
            InvokeOnUIThread(action);
        }
    }
}
