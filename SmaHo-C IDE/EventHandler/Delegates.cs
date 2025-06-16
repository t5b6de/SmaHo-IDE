using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Models.Routing;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.ViewModels.Routing;
using SmaHo_C_IDE.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.EventHandler
{
    // Vorerst hier alle Delegates rein, nachher beim Aufräumen trennen.
    
    /// <summary>
    /// Handler für GateControl Löscung
    /// </summary>
    /// <param name="sender">Gatecontrol, welches gerlöscht wird und dieses ereignis ausgelöst hat</param>
    /// <param name="viewModel">Das verbundene ViewModel des Controls</param>
    public delegate void GateDeletionRequestedEventHandler(LogicGateBaseControl sender, LogicGateBaseViewModel viewModel);

    public delegate void GateViewModelDeletedEventHandler(LogicGateBaseViewModel viewModel, LogicGateBaseModel model);

    public delegate void GateConnectionViewModelDeletedEventHandler(GateConnectionViewModel viewModel, GateConnectionModel model);

}
