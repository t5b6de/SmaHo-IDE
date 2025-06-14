using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Models
{
    class GateConnectionModel
    {
        public int FromGateId { get; set; }
        public int FromOutputIndex { get; set; }

        public int ToGateId { get; set; }
        public int ToInputIndex { get; set; }
    }
}
