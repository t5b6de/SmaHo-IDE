using SmaHo_C_IDE.EventHandler;
using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Models.Routing;
using SmaHo_C_IDE.Services;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.ViewModels.Routing;
using SmaHo_C_IDE.Views.Controls;
using SmaHo_C_IDE.Views.Controls.Routing;
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

        public EditState EditState { get; }

        public Func<LogicGateBaseControl>? GateRequested;

        public event GateConnectionAddedEventHandler ConnectionAdded;
        public event GateViewModelDeletedEventHandler GateViewModelDeleted;
        public event GateConnectionViewModelDeletedEventHandler GateConnectionViewModelDeleted;

        private ObservableCollection<LogicGateBaseViewModel> _GateViewModels = [];
        private ObservableCollection<MultiPolylineConnector> _GateConnections = [];

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
            //_GateConnections.CollectionChanged += GateConnections_CollectionChanged;          

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

                foreach (var item in e.OldItems)
                {
                    if (item is LogicGateBaseViewModel lgbvm)
                    {
                        GateViewModelDeleted?.Invoke(lgbvm, lgbvm.Model);
                        lgbvm.Dispose();

                        // Hier nach Verbindungen suchen und entfernen, TODO: Contains prüfen! ob die Logik hier so passt, unsicher.
                        var toRemove = _GateConnections
                            .Where(c => c.FromViewModel == lgbvm || c.ToViewModels.Contains(lgbvm))
                            .ToList();

                        foreach (var conn in toRemove)
                        {
                            List<GateConnectionViewModel> tmp = new List<GateConnectionViewModel>();
                            var fully = conn.RemovePartial(lgbvm, tmp);
                            if(fully)
                            {
                                _GateConnections.Remove(conn);
                            }

                            foreach(var i in tmp)
                            {
                                GateConnectionViewModelDeleted(i, i.Model);
                                i.Dispose();
                            }
                        }                            
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
            var newConn = new MultiPolylineConnector(_Canvas);
            
            // Einfach durchreichen:
            newConn.ConnectionAdded += ProxyConnectionAdded;

            // TODO hier durchiterieren zwecks hinzufügen.

            if(newConn.AddConnectionToNet(fromEp, toEp))
            {
                _GateConnections.Add(newConn);
            }

        }

        private void ProxyConnectionAdded(GateConnectionModel model)
        {
            ConnectionAdded?.Invoke(model);
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
            // TODO: get Nearest Connection!
            // Dann PreConnectedEntpoint auf IsConnectinLine=true setzen und die PolyLine da drinnen setzen.

            foreach (var gate in _GateViewModels)
            {
                var gatePos = gate.CurrentPosition;

                // inputs
                for (int i = 0; i < gate.InputAnchors.Count(); i++)
                {
                    var ancPos = gatePos + (Vector)gate.InputAnchors[i];

                    if (IsNear(pos, ancPos, cAnchorHitRadius))
                    {
                        return new PreConnectedEndpoint { ViewModel = gate, Index = i, IsOutput = false, IsConnectionLine = false };
                    }
                }

                // outputs
                for (int i = 0; i < gate.OutputAnchors.Count(); i++)
                {
                    var ancPos = gatePos + (Vector)gate.OutputAnchors[i];

                    if (IsNear(pos, ancPos, cAnchorHitRadius))
                    {
                        return new PreConnectedEndpoint { ViewModel = gate, Index = i, IsOutput = true, IsConnectionLine = false };
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
