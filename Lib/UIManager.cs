using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lib
{
    public class UIManager
    {
        private readonly Lib Lib;

        internal UIManager(Lib lib)
        {
            Lib = lib;
        }

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

            if (Lib.TacticalMap == null)
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
                var button = GetControlByName<Button>(Lib.TacticalMap, name);
                Lib.TacticalMap.Activate();
                button.PerformClick();
            });
            Lib.InvokeOnUIThread(action);

            return true;
        }

        public void RunOnForm(AuroraType type, Action<Form> action)
        {
            if (!OpenFormInstance(type))
            {
                Lib.LogWarning($"Could not open form {type}");

                return;
            }

            var formtype = Lib.SignatureManager.Get(AuroraType.EconomicsForm);
            var form = Lib.GetOpenForms().FirstOrDefault(f => f.GetType().Name == formtype.Name);

            var t = new Task(() =>
            {
                var end = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                while (form == null)
                {
                    Lib.LogInfo($"Waiting for form");
                    Thread.Sleep(100);
                    form = Lib.GetOpenForms().FirstOrDefault(f => f.GetType().Name == formtype.Name);

                    if (DateTime.UtcNow > end)
                    {
                        break;
                    }
                }

                if (form == null)
                {
                    Lib.LogWarning($"Could not find open form {type}");

                    return;
                }

                Lib.InvokeOnUIThread(new Action(() =>
                {
                    form.Activate();
                    action(form);
                }));
            });
            t.Start();
        }
    }
}
