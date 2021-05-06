using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;

namespace Lib
{
    public class SignatureManager
    {
        private class Signature
        {
            public AuroraType Name { get; set; }
            public Dictionary<string, bool> IsUniqueByChecksum { get; set; } = new Dictionary<string, bool>();
            public Dictionary<string, int> MinFieldTypes { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, int> MaxFieldTypes { get; set; } = new Dictionary<string, int>();
        }

        private readonly Lib Lib;
        private readonly Dictionary<AuroraType, Signature> Signatures = new Dictionary<AuroraType, Signature>();
        private readonly Dictionary<AuroraType, Type> TypeCache = new Dictionary<AuroraType, Type>();
        public IEnumerable<AuroraType> KnownAuroraTypes => Signatures.Keys;

        public SignatureManager(Lib lib)
        {
            Lib = lib;
            Load();
            GenerateKnownTypes();
        }

        public Type Get(AuroraType name)
        {
            if (TypeCache.ContainsKey(name))
            {
                return TypeCache[name];
            }

            if (Signatures.TryGetValue(name, out Signature signature))
            {
                if (!signature.IsUniqueByChecksum.ContainsKey(Lib.AuroraChecksum))
                {
                    var types = GetTypes(signature);
                    if (types.Count == 1)
                    {
                        signature.IsUniqueByChecksum.Add(Lib.AuroraChecksum, true);
                    }
                    else
                    {
                        signature.IsUniqueByChecksum.Add(Lib.AuroraChecksum, false);
                    }
                }

                if (signature.IsUniqueByChecksum[Lib.AuroraChecksum])
                {
                    var type = GetTypes(signature).First();
                    TypeCache[name] = type;

                    return type;
                }
            }

            return null;
        }

        public void GenerateForType(AuroraType name, Type type)
        {
            var fieldtypes = new Dictionary<Type, int>();

            foreach (var field in type.GetFields(AccessTools.all))
            {
                if (field.FieldType.Assembly != Lib.AuroraAssembly 
                    && field.FieldType.IsGenericType == false
                    && field.FieldType.IsInterface == false)
                {
                    if (!fieldtypes.ContainsKey(field.FieldType))
                    {
                        fieldtypes.Add(field.FieldType, 0);
                    }

                    fieldtypes[field.FieldType]++;
                }
            }

            var signature = new Signature()
            {
                Name = name,
            };

            foreach (var kvp in fieldtypes)
            {
                signature.MinFieldTypes.Add(kvp.Key.Name, kvp.Value - 5);
                signature.MaxFieldTypes.Add(kvp.Key.Name, kvp.Value + 5);
            }

            var types = GetTypes(signature);
            if (types.Count == 1)
            {
                signature.IsUniqueByChecksum.Add(Lib.AuroraChecksum, true);
            }
            else
            {
                signature.IsUniqueByChecksum.Add(Lib.AuroraChecksum, false);
            }

            Signatures[name] = signature;

            Save();
        }

        private void Load()
        {
            Signatures.Clear();

            try
            {
                var signatures = Lib.Deserialize<List<Signature>>("signatures");
                foreach (var signature in signatures)
                {
                    Signatures.Add(signature.Name, signature);
                }
            }
            catch (Exception)
            {
                Lib.Logger.LogInfo("Signatures not found.");
            }
        }

        private void Save()
        {
            var signatures = Signatures.Values.ToList();
            Lib.Serialize("signatures", signatures);
        }

        private List<Type> GetTypes(Signature signature)
        {
            var types = new List<Type>();
            var fieldtypes = new Dictionary<string, int>();

            foreach (var type in Lib.AuroraAssembly.GetTypes())
            {
                fieldtypes.Clear();
                var good = true;

                foreach (var field in type.GetFields(AccessTools.all))
                {
                    var name = field.FieldType.Name;
                    if (!fieldtypes.ContainsKey(name))
                    {
                        fieldtypes.Add(name, 0);
                    }

                    fieldtypes[name]++;
                }

                foreach (var min in signature.MinFieldTypes)
                {
                    var count = fieldtypes.ContainsKey(min.Key) ? fieldtypes[min.Key] : 0;

                    if (count < min.Value)
                    {
                        good = false;
                        break;
                    }
                }

                foreach (var max in signature.MaxFieldTypes)
                {
                    var count = fieldtypes.ContainsKey(max.Key) ? fieldtypes[max.Key] : 0;

                    if (count > max.Value)
                    {
                        good = false;
                        break;
                    }
                }

                if (good)
                {
                    types.Add(type);
                }
            }

            return types;
        }

        private void GenerateKnownTypes()
        {
            if (Lib.AuroraChecksum == "chm1c7")
            {
                GenerateForType(AuroraType.TacticalMapForm, Lib.AuroraAssembly.GetType("jt"));
                GenerateForType(AuroraType.EconomicsForm, Lib.AuroraAssembly.GetType("gz"));
            }
        }
    }
}
