using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Sepidar.Framework
{
    public enum FilterOperation
    {
        [Description("<")]
        LessThan = 0,
        [Description("<=")]
        LessThanEqualTo = 1,
        [Description("=")]
        EuqlaTo = 2,
        [Description("<>")]
        NotEqualTo = 3,
        [Description(">=")]
        GreaterThanEqualTo = 4,
        [Description(">")]
        GreaterThan = 5
    }
}
