using SmaHo_C_IDE.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmaHo_C_IDE.Views.Controls
{
    /// <summary>
    /// Interaktionslogik für LogicOrGate.xaml
    /// </summary>
    public partial class LogicOrGateControl : LogicGateBaseControl
    {
        public LogicOrGateControl(EditState state)
            : base(4, 1, state)
        {
            InitializeComponent();

            //Andockpunkte festlegen
            AddAnchorFromLine(In1, 0, true);
            AddAnchorFromLine(In2, 1, true);
            AddAnchorFromLine(In3, 2, true);
            AddAnchorFromLine(In4, 3, true);

            //Ausgänge:
            AddAnchorFromLine(Out1, 0, false);
        }
    }
}
