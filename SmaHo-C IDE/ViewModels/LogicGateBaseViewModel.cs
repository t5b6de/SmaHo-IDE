using SmaHo_C_IDE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace SmaHo_C_IDE.ViewModels
{
    public abstract class LogicGateBaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public LogicGateBaseModel Model { get; }

        protected Point[] _InputAnchors, _OutputAnchors;
        protected Point _CurrentPosition;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Point CurrentPosition
        {
            get
            {
                return _CurrentPosition;
            }
            set
            {
                this._CurrentPosition = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPosition)));
            }
        }

        //public Point[] InputAnchors { get { return _InputAnchors; } }
        //public Point[] OutputAnchors { get { return _OutputAnchors; } }

        // TODO: ggf. gemeinsames Array mit allen Anchors und nur über Index mit Anzahl ein/ausgänge mappen.
        public Point[] Anchors
        {
            get
            {
                if (_InputAnchors.Length == 0)
                    return _OutputAnchors;

                if(_OutputAnchors.Length == 0)
                    return _InputAnchors;

                return _InputAnchors.Concat(_OutputAnchors).ToArray();
            }
        }

        public LogicGateBaseViewModel(LogicGateBaseModel model)
        {
            Model = model;
            _InputAnchors = [];
            _OutputAnchors = [];
            CurrentPosition = new Point();
        }

        public void InitializePoints(int inCount, int outCount)
        {
            _InputAnchors = new Point[inCount];
            _OutputAnchors = new Point[outCount];
        }

        public void AddAnchorFromLine(Line line, int index, bool isInput)
        {
            var x = line.X1;
            var y = line.Y1;

            if (!isInput)
            {
                x = line.X2;
                y = line.Y2;
            }

            Point[] dst = isInput ? _InputAnchors : _OutputAnchors;
            dst[index] = new Point(x, y);
        }

        /// <summary>
        /// Gibt den "genaueren" Anchor vom "globalen" index zurück
        /// Der Zurückgegebene wert ist der Index des Anchors im jeweiligen Array, Input oder Output
        /// </summary>
        /// <param name="i">golbaler anchor index</param>
        /// <returns>Tupel mit index und typ (true bei Eingang, false bei Ausgang)</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public (int index, bool isInput) GetSpecificAnchorFromIndex(int i)
        {
            if (i < 0 || (i - _InputAnchors.Length) > _OutputAnchors.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(i));
            }

            if (i >= _InputAnchors.Length)
            {
                return (i - _InputAnchors.Length, false);
            }

            return (i, true);
        }

        public void Dispose()
        {

        }
    }
}
