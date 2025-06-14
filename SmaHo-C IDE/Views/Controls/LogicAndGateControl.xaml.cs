using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.ViewModels;
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
    /// Interaktionslogik für LogicAndGate.xaml
    /// </summary>
    public partial class LogicAndGateControl : LogicGateBaseControl
    {
        public LogicAndGateControl(StandardGateViewModel vm)
            : base(vm, 4, 1) // Anzahl Ein/Ausgänge
        {
            InitializeComponent();

            //Andockpunkte festlegen
            vm.AddAnchorFromLine(In1, 0, true);
            vm.AddAnchorFromLine(In2, 1, true);
            vm.AddAnchorFromLine(In3, 2, true);
            vm.AddAnchorFromLine(In4, 3, true);
            
            //Ausgänge:
            vm.AddAnchorFromLine(Out1, 0, false);
        }
    }
}
