using AuroraPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Lib
{
    public class Lib : Patch
    {
        public TypeManager TypeManager { get; private set; } = null;

        protected override void Load(Assembly aurora)
        {
            TypeManager = new TypeManager(aurora, Logger);
        }

        protected override void Start(Form map)
        {

        }
    }
}
