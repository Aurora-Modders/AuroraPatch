using AuroraPatch;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Lib
{
    public class Lib : AuroraPatch.Patch
    {
        public override string Description => "A library of useful features for patch creators.";
        public TypeManager TypeManager { get; private set; } = null;
        public SignatureManager SignatureManager { get; private set; } = null;

        protected override void Load(Harmony harmony)
        {
            TypeManager = new TypeManager(AuroraAssembly, Logger);
            SignatureManager = new SignatureManager(this);
        }
    }
}
