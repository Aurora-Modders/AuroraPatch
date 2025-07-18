using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace AuroraPatch
{
    internal static class Program
    {
        internal static readonly Logger Logger = new Logger();
        internal static Loader Loader { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// Will calculate Aurora.exe checksum, load the assembly, find the TacticalMap, and load up 3rd party patches.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            bool launch = args.Contains("-launch");
            
            var exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Aurora.exe");

            // Handle Aurora.exe path argument (skip -launch when looking for exe path)
            var nonConfigArgs = args.Where(arg => arg != "-launch").ToArray();
            if (nonConfigArgs.Length > 0)
            {
                Logger.LogInfo("User provided Aurora.exe path: " + nonConfigArgs[0]);
                exe = nonConfigArgs[0];
            }
            
            if (!File.Exists(exe))
            {
                Logger.LogCritical($"File {exe} is missing or is not readable.", true);
                Application.Exit();

                return;
            }

            Logger.LogInfo($"Found Aurora at {exe}");

            var checksum = GetChecksum(File.ReadAllBytes(exe));
            Loader = new Loader(exe, checksum);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, a) =>
            {
                foreach (var assembly in ((AppDomain)sender).GetAssemblies())
                {
                    if (assembly.FullName == a.Name)
                    {
                        return assembly;
                    }
                }

                return null;
            };

            var form = new AuroraPatchForm();
            form.Show();
            if (launch)
            {
                // Automatically start Aurora with patches
                form.DoStart();
            }
            Application.Run();
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
