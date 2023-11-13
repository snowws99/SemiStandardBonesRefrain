using PEPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemiStandardBonesRefrain
{
    internal class Option : IPEPluginOption
    {
        public bool Bootup { get { return false; } }

        public bool RegisterMenu { get { return true; } }

        public string RegisterMenuText { get { return "Add Semi-standard Bones (Refrain)"; } }
    }
}
