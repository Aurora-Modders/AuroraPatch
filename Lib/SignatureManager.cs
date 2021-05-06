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
            public string Name { get; set; } = "";
            public Dictionary<string, bool> IsUniqueByChecksum { get; set; } = new Dictionary<string, bool>();
            public int MinFields { get; set; } = 0;
            public int MaxFields { get; set; } = 0;
            public Dictionary<string, int> MinFieldTypes { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, int> MaxFieldTypes { get; set; } = new Dictionary<string, int>();

            public List<Type> GetTypes(TypeManager manager)
            {
                var minfieldtypes = new List<Tuple<Type, int>>();
                var maxfieldtypes = new List<Tuple<Type, int>>();

                foreach (var kvp in MinFieldTypes)
                {
                    minfieldtypes.Add(new Tuple<Type, int>(Type.GetType(kvp.Key), kvp.Value));
                }

                foreach (var kvp in MaxFieldTypes)
                {
                    maxfieldtypes.Add(new Tuple<Type, int>(Type.GetType(kvp.Key), kvp.Value));
                }

                return manager.GetAuroraTypes(minFields: MinFields, maxFields: MaxFields, minFieldTypes: minfieldtypes, maxFieldTypes: maxfieldtypes).ToList();
            }
        }

        private readonly Lib Lib;
        private readonly Dictionary<string, Signature> Signatures = new Dictionary<string, Signature>();
        private readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        public SignatureManager(Lib lib)
        {
            Lib = lib;
        }

        public bool TryGet(string name, out Type type)
        {
            if (TypeCache.ContainsKey(name))
            {
                type = TypeCache[name];

                return true;
            }

            if (Signatures.TryGetValue(name, out Signature signature))
            {
                if (!signature.IsUniqueByChecksum.ContainsKey(Lib.AuroraChecksum))
                {
                    var types = signature.GetTypes(Lib.TypeManager);

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
                    type = signature.GetTypes(Lib.TypeManager).First();
                    TypeCache[name] = type;

                    return true;
                }
            }

            type = default(Type);
            return false;
        }

        public void GenerateForType(string name, Type type)
        {
            var fields = 0;
            var fieldtypes = new Dictionary<Type, int>();

            foreach (var field in type.GetFields(AccessTools.all))
            {
                fields++;

                if (field.FieldType.Assembly != Lib.AuroraAssembly)
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
                MinFields = fields - 5,
                MaxFields = fields + 5
            };

            foreach (var kvp in fieldtypes)
            {
                signature.MinFieldTypes.Add(kvp.Key.Name, kvp.Value - 5);
                signature.MaxFieldTypes.Add(kvp.Key.Name, kvp.Value + 5);
            }

            Signatures[name] = signature;
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
    }
}
