using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace AuroraPatch
{
    public abstract class Patch
    {
        /// <summary>
        /// The names of other patches this one depends on.
        /// </summary>
        protected abstract IEnumerable<string> Dependencies { get; }

        internal void LoadInternal(Assembly aurora)
        {
            Load(aurora);
        }

        internal void StartInternal(Form map)
        {
            Start(map);
        }

        /// <summary>
        /// Called before the game is started. Use this to patch methods etc.
        /// </summary>
        /// <param name="aurora"></param>
        protected abstract void Load(Assembly aurora);

        /// <summary>
        /// Called after game start. You can now invoke code on Aurora's UI thread.
        /// </summary>
        /// /// <param name="map"></param>
        protected abstract void Start(Form map);

        /// <summary>
        /// Run code on Aurora's UI thread. Only available after game start.
        /// </summary>
        protected object InvokeOnUIThread(Delegate method, params object[] args)
        {
            return Program.InvokeOnUIThread(method, args);
        }
    }
}
