using SmaHo_C_IDE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.ViewModels
{
    public abstract class LogicGateBaseViewModel
    {
        public LogicGateBaseModel Model { get; }


        public LogicGateBaseViewModel(LogicGateBaseModel model)
        {
            Model = model;
        }
    }
}
