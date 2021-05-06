using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

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
        protected Form TacticalMap => Loader.TacticalMap;
        
        internal void LoadInternal(Assembly aurora)
        {
            Load(aurora, new Harmony(Name));
        }

        internal void StartInternal()
        {
            Start();
        }

        internal void ChangeSettingsInternal()
        {
            ChangeSettings();
        }

        /// <summary>
        /// Called before the game is started. Use this to patch methods etc. 
        /// TacticalMap will be null and you can not invoke code on Aurora's UI Thread.
        /// </summary>
        /// <param name="aurora"></param>
        protected virtual void Load(Assembly aurora, Harmony harmony)
        {

        }

        /// <summary>
        /// Called after game start. You can now invoke code on Aurora's UI thread and access the TacticalMap.
        /// </summary>
        protected virtual void Start()
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

        /// <summary>
        /// Serialize arbitrary objects, useful for settings.
        /// </summary>
        protected void Serialize<T>(string id, T obj)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            var dir = Path.Combine(Path.GetDirectoryName(AuroraExecutable), "Patches", Name);
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, id + ".json");

            using (var reader = new StreamWriter(file))
            using (var json = new JsonTextWriter(reader))
            {
                serializer.Serialize(json, obj);
            }
        }

        /// <summary>
        /// Deserialize arbitrary objects, useful for settings.
        /// </summary>
        protected T Deserialize<T>(string id)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };

            var file = Path.Combine(Path.GetDirectoryName(AuroraExecutable), "Patches", Name, id + ".json");
            if (!File.Exists(file))
            {
                throw new IOException($"Resource {id} not found");
            }

            using (var reader = new StreamReader(file))
            using (var json = new JsonTextReader(reader))
            {
                return serializer.Deserialize<T>(json);
            }
        }
    }
}
