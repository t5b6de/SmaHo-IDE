using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    class DigitalOutputGateModel : LogicGateBaseModel
    {
        public DigitalOutputGateModel(int id)
            : base(id, GateType.DOut)
        {
        }
    }
}
