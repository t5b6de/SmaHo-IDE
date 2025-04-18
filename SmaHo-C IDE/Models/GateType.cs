using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    public enum GateType
    {
        // EA
        DIn,
        DOut,
        VIn,
        VOut,

        // Standardtypen:
        And,
        Nand,
        Or,
        Nor,
        Xor,
        Not,
        RSFlipFlop,
        TFlipFlop,

        // MDS
        Mds,

        // Verzögerungsglieder
        DelayOn,
        DelayOff,
        DelayOnOff,
        DelayStoreOn,
        DelayPulse,
        DelayTriggerPulse,
        DelayStairLight,
    }
}
