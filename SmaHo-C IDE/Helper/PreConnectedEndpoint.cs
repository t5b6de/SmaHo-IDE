using SmaHo_C_IDE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Helper
{
    public class PreConnectedEndpoint
    {
        public required LogicGateBaseViewModel ViewModel { get; set; }
        public int Index { get; set; }
        public bool IsOutput {  get; set; }
    }
}
