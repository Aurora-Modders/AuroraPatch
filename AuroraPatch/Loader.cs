using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AuroraPatch
{
    internal class Loader
    {
        internal readonly string AuroraExecutable;
        internal readonly string AuroraChecksum;
        internal readonly List<Patch> LoadedPatches = new List<Patch>();
        internal Assembly AuroraAssembly { get; set; } = null;
        internal Form TacticalMap { get; set; } = null;

        private bool Started { get; set; } = false;

        internal Loader(string exe, string checksum)
        {
            AuroraExecutable = exe;
            AuroraChecksum = checksum;
        }

        internal object InvokeOnUIThread(Delegate method, params object[] args)
        {
            if (Started)
            {
                return TacticalMap.Invoke(method, args);
            }
            else
            {
                Program.Logger.LogError("Can not invoke on UI thread before the game is started");

                return null;
            }
        }

        internal List<Patch> FindPatches()
        {
            var patches = new List<Patch>();
            string patchesDirectory = Path.Combine(Path.GetDirectoryName(AuroraExecutable), "Patches");
            Directory.CreateDirectory(patchesDirectory);

            foreach (var dir in Directory.EnumerateDirectories(patchesDirectory))
            {
                foreach (var dll in Directory.EnumerateFiles(dir, "*.dll"))
                {
                    try
                    {
                        foreach (var type in Assembly.LoadFrom(dll).GetTypes())
                        {
                            if (typeof(Patch).IsAssignableFrom(type))
                            {
                                var patch = (Patch)Activator.CreateInstance(type);
                                patch.Loader = this;
                                patches.Add(patch);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Program.Logger.LogDebug($"File {dll} can not be loaded as Assembly.");
                    }
                }
            }

            return patches;
        }

        internal void SortPatches(List<Patch> patches)
        {
            // TODO check for circular dependencies

            patches.Sort((a, b) =>
            {
                if (a.Dependencies.Contains(b.Name))
                {
                    return 1;
                }
                else if (b.Dependencies.Contains(a.Name))
                {
                    return -1;
                }
                else
                {
                    return a.Name.CompareTo(b.Name);
                }
            });
        }

        internal IEnumerable<KeyValuePair<Patch, string>> GetUnmetDependencies(List<Patch> patches)
        {
            var available = new HashSet<string>();
            patches.ForEach(p => available.Add(p.Name));

            foreach (var patch in patches)
            {
                foreach (var dep in patch.Dependencies)
                {
                    if (!available.Contains(dep))
                    {
                        yield return new KeyValuePair<Patch, string>(patch, dep);
                    }
                }
            }
        }

        internal void StartAurora(List<Patch> patches)
        {
            TacticalMap = null;
            LoadedPatches.Clear();
            Started = false;

            Program.Logger.LogInfo("Loading assembly " + AuroraExecutable + " with checksum " + AuroraChecksum);
            AuroraAssembly = Assembly.LoadFile(AuroraExecutable);

            Program.Logger.LogInfo("Loading patches");
            foreach (var patch in patches)
            {
                Program.Logger.LogInfo("Applying patch " + patch.Name);

                patch.LoadInternal();
                LoadedPatches.Add(patch);
            }

            Program.Logger.LogInfo("Starting Aurora");
            TacticalMap = GetTacticalMap(AuroraAssembly);
            TacticalMap.Shown += MapShown;
            TacticalMap.Show();
        }

        /// <summary>
        /// Method called when the TacticalMap Form is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapShown(object sender, EventArgs e)
        {
            Started = true;
            Program.Logger.LogInfo("Starting patches");
            foreach (var patch in LoadedPatches)
            {
                patch.StartInternal();
            }
        }

        /// <summary>
        /// Given the Aurora.exe assembly, find and return the TacticalMap Form object.
        /// This can be a bit tricky due to the obfuscation. We're going in blind and counting buttons/checkboxes.
        /// As of May 3rd 2021 (Aurora 1.13), the TacticalMap had 66 buttons and 68 checkboxes so that's our signature.
        /// We got a bit of wiggle room as we're looking for a Form object with anywhere between 60 and 80 of each.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>TacticalMap Form object</returns>
        private Form GetTacticalMap(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.BaseType.Equals(typeof(Form)))
                {
                    var buttons = 0;
                    var checkboxes = 0;

                    foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (field.FieldType.Equals(typeof(Button)))
                        {
                            buttons++;
                        }
                        else if (field.FieldType.Equals(typeof(CheckBox)))
                        {
                            checkboxes++;
                        }
                    }

                    if (buttons >= 60 && buttons <= 80 && checkboxes >= 60 && checkboxes <= 80)
                    {
                        Program.Logger.LogInfo("TacticalMap found: " + type.Name);
                        var map = (Form)Activator.CreateInstance(type);

                        return map;
                    }
                }
            }

            Program.Logger.LogCritical("TacticalMap not found");
            // If we expose more forms/functionality in the future, may want to make this an error instead
            // and allow execution to continue as some patches may still work if not interfacing with the map.
            throw new Exception("TacticalMap not found");
        }
    }
}
