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

        public Lib()
        {
            KnowledgeBase = new KnowledgeBase(this);
        }

        protected override void Load(Harmony harmony)
        {
            SignatureManager = new SignatureManager(this);
        }

        protected override void Start()
        {
            DatabaseManager = new DatabaseManager(this);
        }
    }
}
