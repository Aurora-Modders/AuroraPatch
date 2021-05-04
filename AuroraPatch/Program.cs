using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace AuroraPatch
{
    public static class Program
    {
        public static string AuroraExecutable { get; private set; } = null;
        public static string AuroraChecksum { get; set; } = null;
        public static Logger Logger = new Logger();

        /// <summary>
        /// The main entry point for the application.
        /// Will calculate Aurora.exe checksum, load the assembly, find the TacticalMap, and load up 3rd party patches.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AuroraExecutable = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Aurora.exe");
            if (args.Length > 0)
            {
                Logger.LogInfo("User provided Aurora.exe path: " + args[0]);
                AuroraExecutable = args[0];
            }
            
            if (!File.Exists(AuroraExecutable))
            {
                Logger.LogCritical($"File {AuroraExecutable} is missing or is not readable.");
                Application.Exit();

                return;
            }

            AuroraChecksum = GetChecksum(File.ReadAllBytes(AuroraExecutable));
            
            Application.Run(new Form1());
        }

        internal static void StartAurora(List<string> files)
        {
            Logger.LogInfo("Loading assembly " + AuroraExecutable + " with checksum " + AuroraChecksum);
            var assembly = Assembly.LoadFile(AuroraExecutable);

            Logger.LogInfo("Retrieving TacticalMap");
            var map = GetTacticalMap(assembly);

            Logger.LogInfo("Loading patches");
            foreach (var file in files)
            {
                foreach (var type in Assembly.LoadFile(file).GetTypes())
                {
                    if (typeof(Patch).IsAssignableFrom(type))
                    {
                        Logger.LogDebug("Applying patch " + type.Name);
                        var patch = (Patch)Activator.CreateInstance(type);
                        patch.Run(map);
                    }
                }
            }

            Logger.LogInfo("Starting Aurora");
            map.Show();
        }

        /// <summary>
        /// Given the Aurora.exe assembly, find and return the TacticalMap Form object.
        /// This can be a bit tricky due to the obfuscation. We're going in blind and counting buttons/checkboxes.
        /// As of May 3rd 2021 (Aurora 1.13), the TacticalMap had 66 buttons and 68 checkboxes so that's our signature.
        /// We got a bit of wiggle room as we're looking for a Form object with anywhere between 60 and 80 of each.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>TacticalMap Form object</returns>
        private static Form GetTacticalMap(Assembly assembly)
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
                        Logger.LogDebug("TacticalMap found: " + type.Name);
                        var map = (Form)Activator.CreateInstance(type);

                        return map;
                    }
                }
            }

            Logger.LogCritical("TacticalMap not found");
            // If we expose more forms/functionality in the future, may want to make this an error instead
            // and allow execution to continue as some patches may still work if not interfacing with the map.
            throw new Exception("TacticalMap not found");
        }

        /// <summary>
        /// Helper method to calculate a byte array checksum.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string GetChecksum(byte[] bytes)
        {
            Logger.LogDebug("Calculating checksum");
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var str = Convert.ToBase64String(hash);

                string checksum = str.Replace("/", "").Replace("+", "").Replace("=", "").Substring(0, 6);
                Logger.LogDebug("Checksum: " + checksum);
                return checksum;
            }
        }
    }
}
