using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inec.StateMachine.WSReferences
{
    public partial class spdRet
    {
        public override string ToString()
        {
            const string template = "code - {0}; error - {1}";
            return string.Format(template, code, error);
        }
    }
}
