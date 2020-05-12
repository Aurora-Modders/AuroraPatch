using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuroraPatch
{
    public abstract class Patch
    {
        private Form TacticalMap { get; set; } = null;

        internal void Run(Form map)
        {
            TacticalMap = map;
            Start(TacticalMap);
        }

        protected abstract void Start(Form map);

        protected void InvokeOnUIThread(Action action)
        {
            TacticalMap.Invoke(action);
        }

        protected object InvokeOnUIThread(Delegate method, params object[] args)
        {
            return TacticalMap.Invoke(method, args);
        }
    }
}
