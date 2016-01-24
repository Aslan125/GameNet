using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameNet.Common.Operations
{
    public enum LoginOperation:byte
    {
        
        InitOk=1<<2,
        AuthLog=1<<3,
        LoginOk=1<<4,
        LoginFail=1<<5
    }
}
