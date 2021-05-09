using HarmonyLib;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automation
{
    public class Automation : AuroraPatch.Patch
    {
        public override IEnumerable<string> Dependencies => new[] { "Lib" };

        protected override void PostStart()
        {
            var lib = GetDependency<Lib.Lib>("Lib");

            foreach (var button in lib.KnowledgeBase.GetTimeIncrementButtons())
            {
                LogInfo($"Button {button.Name} click patched");
                button.Click += OnButtonClick;
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            LogInfo($"You've clicked a time increment button and opened the eco window");

            var lib = GetDependency<Lib.Lib>("Lib");
            var ui = lib.UIManager;

            ui.RunOnForm(AuroraType.EconomicsForm, form =>
            {
                foreach (var c in ui.IterateControls(form))
                {
                    LogInfo($"Control type {c.GetType().Name} name {c.Name}");
                }

                var poptree = ui.GetControlByName<TreeView>(form, "tvPopList");
                foreach (TreeNode system in poptree.Nodes)
                {
                    LogInfo($"System {system.Text}");

                    foreach (TreeNode pop in system.Nodes)
                    {
                        LogInfo($"Pop {pop.Text}");
                    }
                }

                var tabcontrol = ui.GetControlByName<TabControl>(form, "tabPopulation");
                var industrytab = ui.GetControlByName<TabPage>(form, "tabIndustry");
                tabcontrol.SelectedTab = industrytab;

                var constructiontypecombo = ui.GetControlByName<ComboBox>(form, "cboConstructionType");
                foreach (var item in constructiontypecombo.Items)
                {
                    if (item.ToString() == "Components")
                    {
                        constructiontypecombo.SelectedItem = item;
                    }
                }

                var projects = ui.GetControlByName<ListBox>(form, "lstPI");
                LogInfo($"Listbox display member {projects.DisplayMember}");

                foreach (var item in projects.Items.OfType<object>().ToList())
                {
                    var displayed = item.GetType().GetProperty(projects.DisplayMember).GetValue(item, null).ToString();
                    LogInfo($"Listbox item {displayed}");

                    if (displayed == "Diplomacy Module")
                    {
                        projects.SelectedItem = item;
                    }
                }
            });
        }
    }
}
