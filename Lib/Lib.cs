﻿using AuroraPatch;
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
        public UIManager UIManager { get; private set; } // available on Load
        public DatabaseManager DatabaseManager { get; private set; } = null; // available on PostStart

        private static readonly HashSet<Form> OpenForms = new HashSet<Form>();
        private static readonly Dictionary<Type, List<Tuple<string, MethodInfo, Func<Control, bool>>>> EventHandlers = new Dictionary<Type, List<Tuple<string, MethodInfo, Func<Control, bool>>>>();
        private static Lib Instance { get; set; } = null;

        public List<Form> GetOpenForms()
        {
            var forms = new List<Form>();
            lock (OpenForms)
            {
                forms.AddRange(OpenForms);
            }

            return forms;
        }

        public void RegisterEventHandler(AuroraType form, string event_name, MethodInfo handler, Func<Control, bool> predicate = null)
        {
            if (!handler.IsStatic)
            {
                LogError($"Event handler {handler.DeclaringType.Name}.{handler.Name} is not static.");

                return;
            }

            lock (EventHandlers)
            {
                var type = SignatureManager.Get(form);

                if (!EventHandlers.ContainsKey(type))
                {
                    EventHandlers.Add(type, new List<Tuple<string, MethodInfo, Func<Control, bool>>>());
                }

                EventHandlers[type].Add(new Tuple<string, MethodInfo, Func<Control, bool>>(event_name, handler, predicate));
            }
        }

        protected override void Loaded(Harmony harmony)
        {
            Instance = this;
            KnowledgeBase = new KnowledgeBase(this);
            SignatureManager = new SignatureManager(this);
            UIManager = new UIManager(this);

            foreach (var form in AuroraAssembly.GetTypes().Where(t => typeof(Form).IsAssignableFrom(t)))
            {
                try
                {
                    var ctor = (MethodBase)form.GetMember(".ctor", AccessTools.all)[0];
                    var method = new HarmonyMethod(GetType().GetMethod("PostfixFormConstructor", AccessTools.all));
                    harmony.Patch(ctor, null, method);
                }
                catch (Exception e)
                {
                    LogError($"Failed to patch Form constructor {form.Name}, exception: {e}");
                }
            }
        }

        protected override void Started()
        {
            DatabaseManager = new DatabaseManager(this);
        }

        private static void PostfixFormConstructor(Form __instance)
        {
            __instance.Shown += OnFormShown;
            __instance.FormClosing += OnFormFormClosing;

            lock (EventHandlers)
            {
                var key = EventHandlers.Keys.FirstOrDefault(t => t.Name == __instance.GetType().Name);
                if (key == null)
                {
                    return;
                }

                var controls = UIManager.IterateControls(__instance).ToList();

                foreach (var handler in EventHandlers[key])
                {
                    try
                    {
                        if (handler.Item3 == null)
                        {
                            // on form itself
                            AddEventHandler(__instance, handler.Item1, handler.Item2);
                        }
                        else
                        {
                            // on controls on the form
                            foreach (var control in controls.Where(c => handler.Item3(c)))
                            {
                                AddEventHandler(control, handler.Item1, handler.Item2);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Instance.LogError($"Failed to add eventhandler {handler.Item2.DeclaringType.Name}.{handler.Item2.Name}. {e}");
                    }
                }
            }
        }

        private static void OnFormShown(object sender, EventArgs e)
        {
            var form = (Form)sender;
            lock (OpenForms)
            {
                OpenForms.Add(form);
            }
        }

        private static void OnFormFormClosing(object sender, EventArgs e)
        {
            var form = (Form)sender;
            lock (OpenForms)
            {
                OpenForms.Remove(form);
            }
        }

        private static void AddEventHandler(Control control, string event_name, MethodInfo handler)
        {
            var evt = control.GetType().GetEvent(event_name);
            var del = Delegate.CreateDelegate(evt.EventHandlerType, handler);
            evt.AddEventHandler(control, del);
        }
    }
}
