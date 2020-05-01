using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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
            var map = GetTacticalMap(assembly);
            map.Shown += MapShown;

            Application.Run(map);
        }

        private static Form GetTacticalMap(Assembly assembly)
        {
            // 67 buttons
            // 67 checkboxes
            
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
                        Debug.WriteLine("Map: " + type.Name);
                        var map = (Form)Activator.CreateInstance(type);

                        return map;
                    }
                }
            }

            throw new Exception("Tactical Map not found");
        }

        private static void MapShown(object sender, EventArgs e)
        {

        }
    }
}
