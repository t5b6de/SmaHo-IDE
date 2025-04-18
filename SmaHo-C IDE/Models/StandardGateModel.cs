using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    public class StandardGateModel : LogicGateBaseModel
    {
        public StandardGateModel(int id, GateType type)
                   : base(id, type)
        {
        }
    }
}
