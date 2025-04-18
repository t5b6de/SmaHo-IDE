using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    class DigitalInputGateModel : LogicGateBaseModel
    {
        public DigitalInputGateModel(int id)
            : base(id, GateType.DIn)
        {
        }
    }
}
