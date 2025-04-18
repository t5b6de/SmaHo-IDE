using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmaHo_C_IDE.Helper
{
    class GateFactory
    {
        private IdManager _IdMgr;
        public GateFactory(IdManager mgr)
        {
            _IdMgr = mgr;
        }

        public (LogicGateBaseViewModel, LogicGateBaseControl) CreateGate(GateType gt)
        {
            var vm = CreateModelViewModel(gt);
            var m = vm.Model;
            var v = CreateView(gt);

            v.DataContext = vm;

            return (vm, v);
        }

        private LogicGateBaseControl CreateView(GateType gt)
        {
            switch (gt)
            {
                
                default:
                    throw new NotImplementedException();
            }
        }

        private LogicGateBaseViewModel CreateModelViewModel(GateType gt)
        {
            int nextGateId = _IdMgr.GetId(-1, gt);

            switch (gt)
            {
                // TODO bei EA-Typen, Durchreichen des ID-Managers oder Event beim
                // Anpassen der ID, da diese den eigentlichen Ein/Ausgang bestimmt
                case GateType.DIn:
                    return new DigitalInputGateViewModel(new DigitalInputGateModel(nextGateId));

                case GateType.DOut:
                    return new DigitalOutputGateViewModel(new DigitalOutputGateModel(nextGateId));

                case GateType.VIn:
                    return new VirtualInputGateViewModel(new VirtualInputGateModel(nextGateId));

                case GateType.VOut:
                    return new VirtualInputGateViewModel(new VirtualInputGateModel(nextGateId));

                case GateType.And:
                case GateType.Or:
                case GateType.Xor:
                case GateType.Not:
                case GateType.Nand:
                case GateType.Nor:
                case GateType.RSFlipFlop:
                case GateType.TFlipFlop:
                    return new StandardGateViewModel(new StandardGateModel(nextGateId, gt));

                case GateType.Mds:
                    return new MdsGateViewModel(new MdsGateModel(nextGateId));

                case GateType.DelayOn:
                case GateType.DelayOff:
                case GateType.DelayOnOff:
                case GateType.DelayStoreOn:
                case GateType.DelayPulse:
                case GateType.DelayTriggerPulse:
                case GateType.DelayStairLight:
                    return new DelayGateViewModel(new DelayGateModel(nextGateId, gt));

                default:
                    throw new NotImplementedException();
            }
        }

    }
}
