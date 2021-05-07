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
        TacticalMapForm, EconomicsForm, GameState
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
            }
        }

        public object GetGameState(Form map)
        {
            switch (Lib.AuroraChecksum)
            {
                case "chm1c7": return map.GetType().GetField("a", AccessTools.all).GetValue(map);
                default: return null;
            }
        }

        public List<MethodInfo> GetSaveMethods(object game)
        {
            var methods = new List<MethodInfo>();

            foreach (var method in game.GetType().GetMethods(AccessTools.all))
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

            return methods;
        }
    }
}
