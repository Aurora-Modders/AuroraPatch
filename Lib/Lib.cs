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
        public KnowledgeBase KnowledgeBase { get; private set; } = null;
        public SignatureManager SignatureManager { get; private set; } = null;
        public DatabaseManager DatabaseManager { get; private set; } = null;

        private static readonly HashSet<Form> OpenForms = new HashSet<Form>();

        public List<Form> GetOpenForms()
        {
            var forms = new List<Form>();
            lock (OpenForms)
            {
                forms.AddRange(OpenForms);
            }

            return forms;
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

        protected override void Start()
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
