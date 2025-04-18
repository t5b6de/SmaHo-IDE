using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    class MdsGateModel : LogicGateBaseModel
    {
        public MdsGateModel(int id)
            : base(id, GateType.Mds)
        {
        }
    }
}
