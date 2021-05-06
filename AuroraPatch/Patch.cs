using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

namespace AuroraPatch
{
    public abstract class Patch
    {
        public string Name => GetType().Name;
        public virtual string Description { get { return ""; } }
        public virtual IEnumerable<string> Dependencies { get { return Enumerable.Empty<string>(); } }

        internal Loader Loader { get; set; }
        protected string AuroraExecutable => Loader.AuroraExecutable;
        protected string AuroraChecksum => Loader.AuroraChecksum;
        protected Logger Logger => Program.Logger;
        protected IEnumerable<Patch> LoadedPatches => Loader.LoadedPatches;
        
        internal void LoadInternal(Assembly aurora)
        {
            Load(aurora, new Harmony(Name));
        }

        internal void StartInternal(Form map)
        {
            Start(map);
        }

        internal void ChangeSettingsInternal()
        {
            ChangeSettings();
        }

        /// <summary>
        /// Called before the game is started. Use this to patch methods etc.
        /// </summary>
        /// <param name="aurora"></param>
        protected virtual void Load(Assembly aurora, Harmony harmony)
        {

        }

        /// <summary>
        /// Called after game start. You can now invoke code on Aurora's UI thread.
        /// </summary>
        /// /// <param name="map"></param>
        protected virtual void Start(Form map)
        {

        }

        /// <summary>
        /// Called when the user clicks "Change settings."
        /// </summary>
        protected virtual void ChangeSettings()
        {

        }

        /// <summary>
        /// Run code on Aurora's UI thread. Only available after game start.
        /// </summary>
        protected object InvokeOnUIThread(Delegate method, params object[] args)
        {
            return Loader.InvokeOnUIThread(method, args);
        }
    }
}
