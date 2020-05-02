using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AuroraPatch
{
    static class Program
    {
        public static string AuroraExecutableDirectory => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var file = "Aurora.exe";
            if (args.Length > 0)
            {
                file = args[0];
            }

            var assembly = Assembly.LoadFile(Path.Combine(AuroraExecutableDirectory, file));
            var checksum = GetChecksum(File.ReadAllBytes(Path.Combine(AuroraExecutableDirectory, file)));

            List<AuroraVersion> auroraVersions;
            try
            {
                var reader = new JsonTextReader(new StreamReader(Path.Combine(AuroraExecutableDirectory, "versions.json")));
                auroraVersions = JsonSerializer.Create().Deserialize<List<AuroraVersion>>(reader);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Could not read Aurora versions from ""versions.json"".");
                Application.Exit();
                return;
            }

            Form map;
            try
            {
                map = GetTacticalMap(assembly, GetFormType(checksum, auroraVersions));
            }
            catch (Exception)
            {
                MessageBox.Show($@"Tactical Map could not be found inside ""{file}"".");
                Application.Exit();
                return;
            }
            
            map.Shown += MapShown;
            Application.Run(map);
        }

        private static string GetFormType(string checksum, List<AuroraVersion> auroraVersions)
        {
            return auroraVersions.First(version => version.Checksum.Equals(checksum)).FormType;
        }

        private static Form GetTacticalMap(Assembly assembly, string formType)
        {
            var type = assembly.GetType(formType);

            if (type == null || !typeof(Form).IsAssignableFrom(type))
            {
                throw new Exception("Tactical Map not found");
            }
            
            Debug.WriteLine("Map: " + type.Name);
            var map = (Form)Activator.CreateInstance(type);

            return map;
        }

        private static void MapShown(object sender, EventArgs e)
        {
            foreach (var dll in Directory.EnumerateFiles(AuroraExecutableDirectory, "*.Patch.dll"))
            {
                var assembly = Assembly.LoadFile(dll);
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(Patch).IsAssignableFrom(type))
                    {
                        var patch = (Patch)Activator.CreateInstance(type);
                        var thread = new Thread(() => patch.Run((Form)sender)) { IsBackground = true };
                        thread.Start();
                    }
                }
            }
        }

        private static string GetChecksum(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var str = Convert.ToBase64String(hash);
                return str.Replace("/", "").Replace("+", "").Replace("=", "").Substring(0, 6);
            }
        }
    }
}
