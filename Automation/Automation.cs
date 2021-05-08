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

        private Lib.Lib Lib { get; set; } = null;

        public IEnumerable<Control> IterateControls(Control control)
        {
            var stack = new Stack<Control>();
            stack.Push(control);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                yield return current;

                foreach (Control next in current.Controls)
                {
                    stack.Push(next);
                }
            }
        }

        public T GetControlByName<T>(Control parent, string name) where T : Control
        {
            return (T)IterateControls(parent).Single(c => c.Name == name);
        }

        public bool OpenFormInstance(AuroraType type)
        {
            var formtype = Lib.SignatureManager.Get(type);
            if (formtype == null)
            {
                return false;
            }

            foreach (var open in Lib.GetOpenForms())
            {
                if (open.GetType().Name == formtype.Name)
                {
                    return true;
                }
            }

            if (TacticalMap == null)
            {
                return false;
            }

            var name = Lib.KnowledgeBase.GetFormOpenButtonName(type);
            if (name == null)
            {
                return false;
            }

            var action = new Action(() =>
            {
                var button = GetControlByName<Button>(TacticalMap, name);
                TacticalMap.Activate();
                button.PerformClick();
            });
            InvokeOnUIThread(action);

            return true;
        }

        public void RunOnForm(AuroraType type, Action<Form> action)
        {
            if (!OpenFormInstance(type))
            {
                LogWarning($"Could not open form {type}");

                return;
            }

            var formtype = Lib.SignatureManager.Get(AuroraType.EconomicsForm);
            var form = Lib.GetOpenForms().FirstOrDefault(f => f.GetType().Name == formtype.Name);

            var t = new Task(() =>
            {
                var end = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                while (form == null)
                {
                    LogInfo($"Waiting for form");
                    Thread.Sleep(100);
                    form = Lib.GetOpenForms().FirstOrDefault(f => f.GetType().Name == formtype.Name);

                    if (DateTime.UtcNow > end)
                    {
                        break;
                    }
                }

                if (form == null)
                {
                    LogWarning($"Could not find open form {type}");

                    return;
                }

                InvokeOnUIThread(new Action(() =>
                {
                    form.Activate();
                    action(form);
                }));
            });
            t.Start();
        }

        protected override void Load(Harmony harmony)
        {
            Lib = (Lib.Lib)LoadedPatches.Single(p => p.Name == "Lib");
        }

        protected override void Start()
        {
            foreach (var button in Lib.KnowledgeBase.GetTimeIncrementButtons())
            {
                LogInfo($"Button {button.Name} click patched");
                button.Click += OnButtonClick;
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            LogInfo($"You've clicked a time increment button and opened the eco window");

            RunOnForm(AuroraType.EconomicsForm, form =>
            {
                LogControlsInfo(form);

                var poptree = GetControlByName<TreeView>(form, "tvPopList");
                foreach (TreeNode system in poptree.Nodes)
                {
                    LogInfo($"System {system.Text}");

                    foreach (TreeNode pop in system.Nodes)
                    {
                        LogInfo($"Pop {pop.Text}");
                    }
                }

                var tabcontrol = GetControlByName<TabControl>(form, "tabPopulation");
                var industrytab = GetControlByName<TabPage>(form, "tabIndustry");
                tabcontrol.SelectedTab = industrytab;

                var constructiontypecombo = GetControlByName<ComboBox>(form, "cboConstructionType");
                foreach (var item in constructiontypecombo.Items)
                {
                    if (item.ToString() == "Components")
                    {
                        constructiontypecombo.SelectedItem = item;
                    }
                }

                var projects = GetControlByName<ListBox>(form, "lstPI");
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

        private void LogControlsInfo(Control control)
        {
            foreach (var c in IterateControls(control))
            {
                LogInfo($"Control type {c.GetType().Name} name {c.Name}");
            }
        }
    }
}
