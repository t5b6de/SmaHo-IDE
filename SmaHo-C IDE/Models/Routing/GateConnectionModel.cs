using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models.Routing
{
    public class GateConnectionModel
    {
        public int FromGateId { get; set; }
        public int FromIndex { get; set; }

        public int ToGateId { get; set; }
        public int ToIndex { get; set; }
    }
}
