using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.EventHandler
{
    // public delegate void PropertyChangedEventHandler(object? sender, PropertyChangedEventArgs e);
    
    /// <summary>
    /// Handler für GateControl Löscung
    /// </summary>
    /// <param name="sender">Gatecontrol, welches gerlöscht wird und dieses ereignis ausgelöst hat</param>
    /// <param name="viewModel">Das verbundene ViewModel des Controls</param>
    public delegate void GateDeletionRequestedEventHandler(LogicGateBaseControl sender, LogicGateBaseViewModel viewModel);

    public delegate void GateViewModelDeletedEventHandler(LogicGateBaseViewModel viewModel, LogicGateBaseModel model);

}
