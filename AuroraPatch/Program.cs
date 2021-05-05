using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace AuroraPatch
{
    /// <summary>
    /// Our patcher program.
    /// </summary>
    public static class Program
    {
        public static string AuroraExecutableDirectory => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public static string AuroraChecksum { get; set; } = null;
        public static TypeManager TypeManager;
        public static Logger logger;

        /// <summary>
        /// The main entry point for the application.
        /// Will calculate Aurora.exe checksum, load the assembly, find the TacticalMap, and load up 3rd party patches.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // TODO: Parse CLI flags for log settings.
            logger = new Logger(LogLevel.Info, "AuroraPatch", "AuroraPatch.log");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var file = "Aurora.exe";
            if (args.Length > 0)
            {
                logger.LogInfo("User provided Aurora.exe path: " + args[0]);
                file = args[0];
            }
            
            var auroraExecutableFullPath = Path.Combine(AuroraExecutableDirectory, file);
            if (!File.Exists(auroraExecutableFullPath))
            {
                logger.LogCritical($@"File ""{file}"" is missing or is not readable.");
                Application.Exit();
                return;
            }

            AuroraChecksum = GetChecksum(File.ReadAllBytes(auroraExecutableFullPath));
            logger.LogInfo("Loading assembly " + auroraExecutableFullPath + " with checksum " + AuroraChecksum);
            var auroraAssembly = Assembly.LoadFile(auroraExecutableFullPath);

            logger.LogInfo("Finding and storing Aurora Form types");
            TypeManager = new TypeManager(auroraAssembly, logger);

            var map = TypeManager.GetFormInstance(AuroraFormType.TacticalMap);
            if (map == null)
            {
                logger.LogCritical("Failed to create TacticalMap Form instance - can not patch Aurora executable");
                throw new Exception("TacticalMap not found");
            }

            map.Shown += MapShown;
            logger.LogInfo("Loading patches and starting Aurora");
            Application.Run(map);
        }

        /// <summary>
        /// Method called when the TacticalMap Form is shown.
        /// We use this opportunity to load up the various 3rd party patches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MapShown(object sender, EventArgs e)
        {
            logger.LogDebug("MapShown callback method - searching for 3rd party patches ending in *.Patch.dll in " + AuroraExecutableDirectory);
            foreach (var dll in Directory.EnumerateFiles(AuroraExecutableDirectory, "*.Patch.dll"))
            {
                var assembly = Assembly.LoadFile(dll);
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(Patch).IsAssignableFrom(type))
                    {
                        logger.LogDebug("Found " + type.Name + " - creating instance and starting in background thread");
                        var patch = (Patch)Activator.CreateInstance(type);
                        var thread = new Thread(() => patch.Run(TypeManager)) { IsBackground = true };
                        thread.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Helper method to calculate a byte array checksum.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string GetChecksum(byte[] bytes)
        {
            logger.LogDebug("Calculating checksum");
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var str = Convert.ToBase64String(hash);

                string checksum = str.Replace("/", "").Replace("+", "").Replace("=", "").Substring(0, 6);
                logger.LogDebug("Checksum: " + checksum);
                return checksum;
            }
        }
    }
}
