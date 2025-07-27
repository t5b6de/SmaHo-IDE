using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.Views.Controls.Routing;
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

        public bool IsConnectionLine { get; set; }

        // Polyline wenn isConnectinLine = true
        public MultiPolylineConnector ConnectionLine { get; set; }

    }
}
