using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Lib
{
    public enum AuroraType
    {
        TacticalMapForm, EconomicsForm, GameState, ClassDesignForm, CreateProjectForm, FleetWindowForm, 
        MissileDesignForm, TurretDesignForm, GroundUnitDesignForm, CommandersWindowForm, MedalsForm, 
        RaceWindowForm, SystemViewForm, GalacticMapForm, RaceComparisonForm, DiplomacyForm, TechnologyViewForm, 
        MineralsForm, SectorsForm, EventsForm, GameDetailsForm
    }

    public class KnowledgeBase
    {
        private readonly Lib Lib;

        internal KnowledgeBase(Lib lib)
        {
            Lib = lib;
        }

        public IEnumerable<KeyValuePair<AuroraType, string>> GetKnownTypeNames()
        {
            if (Lib.AuroraChecksum == "chm1c7")
            {
                yield return new KeyValuePair<AuroraType, string>(AuroraType.TacticalMapForm, "jt");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.EconomicsForm, "gz");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.GameState, "aw");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.ClassDesignForm, "a8");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.CreateProjectForm, "a2");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.FleetWindowForm, "fs");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.MissileDesignForm, "g2");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.TurretDesignForm, "j2");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.GroundUnitDesignForm, "gg");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.CommandersWindowForm, "az");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.MedalsForm, "a4");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.RaceWindowForm, "hw");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.SystemViewForm, "js");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.GalacticMapForm, "a6");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.RaceComparisonForm, "hu");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.DiplomacyForm, "a0");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.TechnologyViewForm, "j1");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.MineralsForm, "g1");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.SectorsForm, "a5");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.EventsForm, "ff");
                yield return new KeyValuePair<AuroraType, string>(AuroraType.GameDetailsForm, "i2");
            }
        }

        public object GetGameState(Form map)
        {
            var type = Lib.SignatureManager.Get(AuroraType.GameState);
            if (type == null)
            {
                return null;
            }

            try
            {
                foreach (var field in map.GetType().GetFields(AccessTools.all))
                {
                    if (field.FieldType.Name != type.Name)
                    {
                        continue;
                    }
                    Lib.LogDebug($"GameState field {field.Name}");

                    return field.GetValue(map);
                }
            }
            catch (Exception e)
            {
                Lib.LogError($"KnowledgeBase failed to retrieve GameState. {e}");
            }
            

            return null;
        }

        public List<MethodInfo> GetSaveMethods()
        {
            var methods = new List<MethodInfo>();

            var type = Lib.SignatureManager.Get(AuroraType.GameState);
            if (type == null)
            {
                return methods;
            }

            try
            {
                foreach (var method in type.GetMethods(AccessTools.all))
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length != 1)
                    {
                        continue;
                    }

                    if (parameters[0].ParameterType.Name != "SQLiteConnection")
                    {
                        continue;
                    }

                    methods.Add(method);
                }
            }
            catch (Exception e)
            {
                Lib.LogError($"KnowledgeBase failed to retrieve save methods. {e}");
            }

            return methods;
        }

        public List<Button> GetTimeIncrementButtons()
        {
            var buttons = new List<Button>();

            if (Lib.TacticalMap == null)
            {
                return buttons;
            }

            var stack = new Stack<Control>();
            stack.Push(Lib.TacticalMap);

            while (stack.Count > 0)
            {
                var control = stack.Pop();

                if (control is Button button)
                {
                    if (button.Name.Contains("cmdIncrement"))
                    {
                        buttons.Add(button);
                    }
                }

                foreach (Control c in control.Controls)
                {
                    stack.Push(c);
                }
            }

            return buttons;
        }

        public string GetFormOpenButtonName(AuroraType type)
        {
            switch (type)
            {
                case AuroraType.EconomicsForm: return "cmdToolbarColony";
                case AuroraType.ClassDesignForm: return "cmdToolbarClass";
                case AuroraType.CreateProjectForm: return "cmdToolbarProject";
                case AuroraType.FleetWindowForm: return "cmdToolbarFleet";
                case AuroraType.MissileDesignForm: return "cmdToolbarMissileDesign";
                case AuroraType.TurretDesignForm: return "cmdToolbarTurret";
                case AuroraType.GroundUnitDesignForm: return "cmdToolbarGroundForces";
                case AuroraType.CommandersWindowForm: return "cmdToolbarCommanders";
                case AuroraType.MedalsForm: return "cmdToolbarMedals";
                case AuroraType.RaceWindowForm: return "cmdToolbarRace";
                case AuroraType.SystemViewForm: return "cmdToolbarSystem";
                case AuroraType.GalacticMapForm: return "cmdToolbarGalactic";
                case AuroraType.RaceComparisonForm: return "cmdToolbarComparison";
                case AuroraType.DiplomacyForm: return "cmdToolbarIntelligence";
                case AuroraType.TechnologyViewForm: return "cmdToolbarTechnology";
                case AuroraType.MineralsForm: return "cmdToolbarSurvey";
                case AuroraType.SectorsForm: return "cmdToolbarSector";
                case AuroraType.EventsForm: return "cmdToolbarEvents";
                case AuroraType.GameDetailsForm: return "cmdToolbarGame";
                default: return null;
            }
        }
    }
}
