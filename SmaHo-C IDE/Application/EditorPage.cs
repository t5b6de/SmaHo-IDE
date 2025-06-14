using SmaHo_C_IDE.EventHandler;
using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Services;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmaHo_C_IDE.Application
{
    // Quasi ViewModel für den Editor
    class EditorPage
    {
        public int Id { get; }
        public string Title { get; set; }
        public string Description { get; set; }

        private const double cAnchorHitRadius = 8.0;
        private const int cMaxOutputUsage = 15;

        public EditState EditState { get; }

        public Func<LogicGateBaseControl>? GateRequested;

        public event Action<GateConnectionModel>? ConnectionAdded;
        public event GateViewModelDeletedEventHandler GateViewModelDeleted;

        private ObservableCollection<LogicGateBaseViewModel> _GateViewModels = new ObservableCollection<LogicGateBaseViewModel>();
        private List<GateConnectionViewModel> _GateConnections = new List<GateConnectionViewModel>();

        private Canvas _Canvas { get; }

        // Point _StartDragPosition;
        bool _IsDragging = false;
        Line _TemporaryLine = new Line();


        public EditorPage(int id, Canvas c)
        {
            _Canvas = c;
            Id = id;
            EditState = new EditState();

            // TODO Maushändigkeit anpassen!
            c.MouseLeftButtonDown += CanvasOnMouseDown;
            c.MouseMove += CanvasOnMouseMove;
            c.MouseLeftButtonUp += CanvasOnMouseUp;

            Title = "";
            Description = "";

            _GateViewModels.CollectionChanged += GateViewModels_CollectionChanged;

            // _StartDragPosition = new Point(0, 0);
            _TemporaryLine.StrokeThickness = 1;
            _TemporaryLine.Stroke = new SolidColorBrush(Colors.Black);
        }

        private void GateViewModels_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems == null) // prevent null warning
                    return;

                foreach(var item in e.OldItems)
                {
                    if(item is LogicGateBaseViewModel lgbvm)
                    {
                        GateViewModelDeleted?.Invoke(lgbvm, lgbvm.Model);
                    }
                }
            }
        }

        private void CanvasOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!EditState.IsActivated)
                return;

            if (_IsDragging)
            {
                _Canvas.Children.Remove(_TemporaryLine);
                _IsDragging = false;

                PreConnectedEndpoint anc1, anc2;

                var tmp = GetNearestAnchor(new Point(_TemporaryLine.X1, _TemporaryLine.Y1));
                //var anc2 = GetNearestAnchor(new Point(_TemporaryLine.X2, _TemporaryLine.Y2));

                if (tmp == null)
                    return;

                anc1 = tmp;

                tmp = GetNearestAnchor(new Point(_TemporaryLine.X2, _TemporaryLine.Y2));

                if (tmp == null)
                    return;

                anc2 = tmp;

                // korrekt durchtauschen, das anc1 immer der output ist, prüfung ob falsch in AddConnectino funktion.
                if (!anc1.IsOutput)
                {
                    tmp = anc2;
                    anc2 = anc1;
                    anc1 = tmp;
                }

                AddConnectionToPage(anc1, anc2);
            }
        }

        private void AddConnectionToPage(PreConnectedEndpoint fromEp, PreConnectedEndpoint toEp)
        {
            // Prüfung: 
            if (fromEp.IsOutput == toEp.IsOutput) // dann Eingang <-> Eingang oder Ausgang <-> Ausgang -> nicht zulässig.
                return;

            if (fromEp.ViewModel == toEp.ViewModel) // direkte Rückkopplung (stand jetzt) möglich aber unerwünscht.
                return;

            int outCount = 0;

            // grundsätzliche Prüfung, 1 Eingang darf nicht mit mehr als 1 Ausgang verbunden sein:
            foreach (GateConnectionViewModel gcvm in _GateConnections)
            {
                if (gcvm.Identical(toEp)) // Multibelegung Eingang - damit wird exakte Konfiguration ebenfalls unterbunden
                    return;

                // zählen wie oft Ausgang bereits verwendet, ist begrenzt:
                if(gcvm.Identical(fromEp))
                {
                    outCount++;

                    if (outCount >= cMaxOutputUsage)
                        return;
                }
            }

            // Dnn sollte (vorerst) alles so passen, nun die Models und Darstellung erzeugen.
            GateConnectionModel gcm = new GateConnectionModel();

            gcm.FromGateId = fromEp.ViewModel.Model.Id;
            gcm.FromOutputIndex = fromEp.Index;
            gcm.ToGateId = toEp.ViewModel.Model.Id;
            gcm.ToInputIndex = toEp.Index;

            GateConnectionViewModel cvm = new GateConnectionViewModel(gcm, fromEp.ViewModel, toEp.ViewModel);

            // Hier nun Linie erzeugen:
            Line connLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            // Binding für X1
            connLine.SetBinding(Line.X1Property, new Binding("Start.X")
            {
                Source = cvm,
                Mode = BindingMode.OneWay
            });

            // Binding für Y1
            connLine.SetBinding(Line.Y1Property, new Binding("Start.Y")
            {
                Source = cvm,
                Mode = BindingMode.OneWay
            });

            // Binding für X2
            connLine.SetBinding(Line.X2Property, new Binding("End.X")
            {
                Source = cvm,
                Mode = BindingMode.OneWay
            });

            // Binding für Y2
            connLine.SetBinding(Line.Y2Property, new Binding("End.Y")
            {
                Source = cvm,
                Mode = BindingMode.OneWay
            });

            _Canvas.Children.Add(connLine);

            _GateConnections.Add(cvm);
            ConnectionAdded?.Invoke(gcm);
        }

        private void CanvasOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!EditState.IsActivated)
                return;

            if (EditState.EditMode == EditMode.Connect)
            {
                if (_IsDragging)
                {
                    var position = e.GetPosition(_Canvas);

                    _TemporaryLine.X2 = position.X;
                    _TemporaryLine.Y2 = position.Y;
                }
            }

        }

        private void CanvasOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!EditState.IsActivated)
                return;

            var pos = e.GetPosition(_Canvas);

            if (EditState.EditMode == EditMode.AddGate)
            {
                if (GateRequested == null)
                    return;

                LogicGateBaseControl ct = GateRequested();

                ct.EditState = EditState;
                ct.DeletionRequested += GateDeletionHandler; ;

                _GateViewModels.Add(ct.ViewModel);
                _Canvas.Children.Add(ct);

                ct.SetPosition(pos.X - (ct.Width / 2), pos.Y - (ct.Height / 2));
            }
            else if (EditState.EditMode == EditMode.Connect)
            {
                // hier dann Linien-Zeichnen
                // _StartDragPosition = pos;
                _IsDragging = true;

                _TemporaryLine.X1 = pos.X;
                _TemporaryLine.Y1 = pos.Y;
                _TemporaryLine.X2 = pos.X;
                _TemporaryLine.Y2 = pos.Y;

                _Canvas.Children.Add(_TemporaryLine);

                // ablauf:
                // hier neue Linie erstellen, einfaches dünnes "line" objekt
                // dieses dann von Startpunkt bis Mauszeigerposition rendern.
                // bei Mouse Move Position anpassen, von der 2. xy-koordinate.
                // bei Mouse-Up entsprechend alles prüfen, wenn positionen im vorbestimmten Radius 
                // übereinstimmen, dann dauerhafte Verbindung zeichnen, Models usw. erstlelen

            }
        }

        private void GateDeletionHandler(LogicGateBaseControl sender, LogicGateBaseViewModel viewModel)
        {
            _Canvas.Children.Remove(sender);
            _GateViewModels.Remove(viewModel);
        }

        private PreConnectedEndpoint? GetNearestAnchor(Point pos)
        {
            foreach (var gate in _GateViewModels)
            {
                var gatePos = gate.CurrentPosition;

                // inputs
                for (int i = 0; i < gate.InputAnchors.Count(); i++)
                {
                    var ancPos = gatePos + (Vector)gate.InputAnchors[i];

                    if (IsNear(pos, ancPos, cAnchorHitRadius))
                    {
                        return new PreConnectedEndpoint { ViewModel = gate, Index = i, IsOutput = false };
                    }
                }

                // outputs
                for (int i = 0; i < gate.OutputAnchors.Count(); i++)
                {
                    var ancPos = gatePos + (Vector)gate.OutputAnchors[i];

                    if (IsNear(pos, ancPos, cAnchorHitRadius))
                    {
                        return new PreConnectedEndpoint { ViewModel = gate, Index = i, IsOutput = true };
                    }
                }
            }

            return null;
        }

        private bool IsNear(Point a, Point b, double distance)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (dx * dx + dy * dy) <= (distance * distance);
        }

    }
}
