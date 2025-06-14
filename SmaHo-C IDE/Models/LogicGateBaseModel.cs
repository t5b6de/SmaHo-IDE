using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    public abstract class LogicGateBaseModel
    {
        public int Id { get; private set; }
        public GateType GateType { get; private set; }
        protected LogicGateBaseModel(int id, GateType type)
        {
            Id = id;
            GateType = type;

        }

    }
}
