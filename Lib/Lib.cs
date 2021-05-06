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

        protected override void Load(Assembly aurora, Harmony harmony)
        {
            TypeManager = new TypeManager(aurora, Logger);
        }

        protected override void Start(Form map)
        {

        }
    }
}
