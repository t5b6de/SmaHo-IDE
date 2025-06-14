using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmaHo_C_IDE.ViewModels
{
    class GateConnectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public GateConnectionModel Model { get; }

        // Zugriff auf Gate-ViewModels:
        private readonly LogicGateBaseViewModel _fromGate;
        private readonly LogicGateBaseViewModel _toGate;

        /// <summary>
        /// ViewModel des Gates von dem verbunden wird
        /// </summary>
        public LogicGateBaseViewModel FromViewModel
            { get { return _fromGate; } }

        /// <summary>
        /// ViewModel des Gates zu dem verbunden wird
        /// </summary>
        public LogicGateBaseViewModel ToViewModel
            { get { return _toGate; } }

        /// <summary>
        /// Grafischer Startpunkt innerhalb des Canvas, von dem verbunden wird
        /// </summary>
        public Point Start
        {
            get
            {
                return _fromGate.CurrentPosition + (Vector)_fromGate.OutputAnchors[Model.FromOutputIndex];
            }
        }

        /// <summary>
        /// Grafischer Endpunkt innerhalb des Canvas, zu dem verbunden wird
        /// </summary>
        public Point End
        {
            get
            {
                return _toGate.CurrentPosition + (Vector)_toGate.InputAnchors[Model.ToInputIndex];
            }
        }

        public GateConnectionViewModel(GateConnectionModel model, LogicGateBaseViewModel start, LogicGateBaseViewModel end)
        {
            Model = model;
            _fromGate = start;
            _toGate = end;

            _fromGate.PropertyChanged += FromGateChanged;
            _toGate.PropertyChanged += ToGateChanged;
        }

        public bool Identical(PreConnectedEndpoint source)
        {
            if (source.IsOutput)
            {
                if (_fromGate != source.ViewModel)
                    return false;

                if (Model.FromOutputIndex != source.Index)
                    return false;
            }
            else
            {
                if (_toGate != source.ViewModel)
                    return false;

                if (Model.ToInputIndex != source.Index)
                    return false;
            }

            return true;
        }

        private void ToGateChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(End)));
        }

        private void FromGateChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Start)));
        }
    }
}
