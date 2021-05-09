using AuroraPatch;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Lib
{
    public class Lib : AuroraPatch.Patch
    {
        public override string Description => "A library of useful features for patch creators.";
        public KnowledgeBase KnowledgeBase { get; private set; } = null; // available on Load
        public SignatureManager SignatureManager { get; private set; } = null; // available on Load
        public DatabaseManager DatabaseManager { get; private set; } = null; // available on PostStart

        private static readonly HashSet<Form> OpenForms = new HashSet<Form>();
        private static readonly Dictionary<AuroraType, List<Tuple<string, MethodInfo, string>>> EventHandlers = new Dictionary<AuroraType, List<Tuple<string, MethodInfo, string>>>();
        private static volatile bool CanRegisterEventHandlers = true;

        public List<Form> GetOpenForms()
        {
            var forms = new List<Form>();
            lock (OpenForms)
            {
                forms.AddRange(OpenForms);
            }

            return forms;
        }

        public void RegisterEventHandler(AuroraType form, string event_name, MethodInfo handler, string control_name = null)
        {
            if (!CanRegisterEventHandlers)
            {
                return;
            }

            lock (EventHandlers)
            {
                if (!EventHandlers.ContainsKey(form))
                {
                    EventHandlers.Add(form, new List<Tuple<string, MethodInfo, string>>());
                }

                EventHandlers[form].Add(new Tuple<string, MethodInfo, string>(event_name, handler, control_name));
            }
        }

        protected override void Load(Harmony harmony)
        {
            KnowledgeBase = new KnowledgeBase(this);
            SignatureManager = new SignatureManager(this);

            foreach (var form in AuroraAssembly.GetTypes().Where(t => typeof(Form).IsAssignableFrom(t)))
            {
                var ctor = (MethodBase)form.GetMember(".ctor", AccessTools.all)[0];
                var method = new HarmonyMethod(GetType().GetMethod("PostfixFormConstructor", AccessTools.all));
                harmony.Patch(ctor, null, method);
            }
        }

        protected override void PreStart(Harmony harmony)
        {
            CanRegisterEventHandlers = false;


        }

        protected override void PostStart()
        {
            DatabaseManager = new DatabaseManager(this);
        }

        private static void PostfixFormConstructor(Form __instance)
        {
            __instance.Shown += OnFormShown;
            __instance.FormClosing += OnFormFormClosing;
        }

        private static void OnFormShown(object sender, EventArgs e)
        {
            lock (OpenForms)
            {
                OpenForms.Add((Form)sender);
            }
        }

        private static void OnFormFormClosing(object sender, EventArgs e)
        {
            lock (OpenForms)
            {
                OpenForms.Remove((Form)sender);
            }
        }
    }
}
