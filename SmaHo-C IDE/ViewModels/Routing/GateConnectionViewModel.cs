using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Models.Routing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmaHo_C_IDE.ViewModels.Routing
{

    // Polyline kann nicht erweitert werden, daher hier auch teilweise Control-Logik enthalten.

    public class GateConnectionViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public GateConnectionModel Model { get; }

        // Zugriff auf Gate-ViewModels:
        private readonly LogicGateBaseViewModel _fromGate;
        private readonly LogicGateBaseViewModel _toGate;

        private ObservableCollection<Point> _intermediatePoints = new();

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

        public List<Point> Points
        {
            get
            {
                List<Point> p = new List<Point>();

                p.Add(Start);
                p.AddRange(_intermediatePoints);
                p.Add(End);

                return p;
            }
        }

        public GateConnectionViewModel(GateConnectionModel model, LogicGateBaseViewModel start, LogicGateBaseViewModel end)
        {
            Model = model;
            _fromGate = start;
            _toGate = end;

            _fromGate.PropertyChanged += FromGateChanged;
            _toGate.PropertyChanged += ToGateChanged;
            _intermediatePoints.CollectionChanged += IntermediatePointsChanged;

            // Vertikale Zwischenlinie einfügen:
            double tmp = Start.X + ((End.X - Start.X) / 2);
            _intermediatePoints.Add(new Point(tmp, Start.Y));
            _intermediatePoints.Add(new Point(tmp, End.Y));
        }

        private void IntermediatePointsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points)));
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

            var tmp = _intermediatePoints[_intermediatePoints.Count() - 1];
            tmp.Y = End.Y;

            _intermediatePoints[_intermediatePoints.Count() - 1] = tmp;
        }

        private void FromGateChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Start)));

            var tmp = _intermediatePoints[0];
            tmp.Y = Start.Y;

            _intermediatePoints[0] = tmp;
        }
        public void Dispose()
        {
            _fromGate.PropertyChanged -= FromGateChanged;
            _toGate.PropertyChanged -= ToGateChanged;
            _intermediatePoints.CollectionChanged -= IntermediatePointsChanged;
            _intermediatePoints.Clear();
        }
    }
}
