using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQF.Tools.IISManager
{
    public enum ServerState
    {
        Unknown = 0,
        Starting = 1,
        Started = 2,
        Stopping = 3,
        Stopped = 4,
        Pausing = 5,
        Paused = 6,
        Continuing = 7
    }
}
