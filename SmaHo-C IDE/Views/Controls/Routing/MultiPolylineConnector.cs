using SmaHo_C_IDE.EventHandler;
using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Models.Routing;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.ViewModels.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmaHo_C_IDE.Views.Controls.Routing
{
    /// <summary>
    /// Klasse zur Interaktionslogik und Sammlung multipler Verbindungslinien (single input multiple output)
    /// ToDo, ggf. kann man hierüber auch Verbindung Seitenübergreifend bauen.
    /// </summary>
    public class MultiPolylineConnector
    {
        private List<GateConnectionViewModel> _GateConnections;
        private List<Polyline> _Lines;


        private Canvas _Canvas;
        private const int cMaxOutputUsage = 15;

        public event GateConnectionAddedEventHandler ConnectionAdded;

        public LogicGateBaseViewModel FromViewModel
        {
            get
            {
                return _GateConnections[0].FromViewModel;
            }
        }

        public LogicGateBaseViewModel[] ToViewModels
        {
            get
            {
                List<LogicGateBaseViewModel> ret = [];
                foreach (var gateConnectionViewModel in _GateConnections)
                    ret.Add(gateConnectionViewModel.ToViewModel);

                return [.. ret];
            }
        }

        public MultiPolylineConnector(Canvas c)
        {
            _Canvas = c;
            _Lines = new List<Polyline>();
            _GateConnections = new List<GateConnectionViewModel>();
        }

        /// <summary>
        /// Entfernt betroffenen Teil vom Canvas. War dieses der letzte Teil, wird true zurückgegeben.
        /// </summary>
        /// <param name="fromEp">start-Endpunkt</param>
        /// <param name="toEp">Ziel-Endpunkt</param>
        /// <returns>true, wenn alles entfernt, false, wenn noch verbleibende Linien vorhanden sind.</returns>
        public bool RemovePartial(LogicGateBaseViewModel removedModel, List<GateConnectionViewModel> deleted = null)
        {
            List<GateConnectionViewModel> toDelete = [];
            List<Polyline> toDeletePolys = [];

            foreach (GateConnectionViewModel gcvm in _GateConnections)
            {
                if (gcvm.FromViewModel != removedModel && gcvm.ToViewModel != removedModel)
                    continue;

                toDelete.Add(gcvm);

                if(deleted != null)
                {
                    deleted.Add(gcvm);
                }

                foreach (UIElement poly in _Canvas.Children)
                {
                    if (poly is Polyline l)
                    {
                        if (l.DataContext == gcvm)
                            toDeletePolys.Add(l);
                    }
                }
            }

            foreach (var poly in toDeletePolys)
                _Canvas.Children.Remove(poly);

            foreach (var conn in toDelete)
                _GateConnections.Remove(conn);

            return _GateConnections.Count == 0;
        }

        // TOOD Idee:
        // Beim Verbinden weiterer Gates mit bestehender Verbindung:
        // Verbindung nicht neu erstellen, sondern dieser hinzufügen, um unnötige Verbindungen zu vermeiden.
        // Am Verbindungspunkt dann die Linie auftrennen und einen Verbindungs-Dot setzen.

        public bool AddConnectionToNet(PreConnectedEndpoint fromEp, PreConnectedEndpoint toEp)
        {
            // TODO: Zwischenstände zulassen, wie 2 Verbundene Eingänge, die dann durch eine weitere Linie mit
            // einem Ausgang verbunden werden (evtl. rot einfärben), sonst ist das Zeichnen nicht sehr Intuitiv

            // Prüfung: 
            if (fromEp.IsOutput && toEp.IsOutput) // zwei Ausgänge dürfen nicht miteinander verbunden werden
                return false;

            if(fromEp.IsConnectionLine && toEp.IsConnectionLine) // keine zwei ConnectionLines zusammen
                return false;

            // wenn einer der EPs ein Connectionline ist, dann darf es nur diese sein.
            if (fromEp.IsConnectionLine && fromEp.ConnectionLine != null && fromEp.ConnectionLine != this)
                return false;
         
            if (toEp.IsConnectionLine && toEp.ConnectionLine != null && toEp.ConnectionLine != this)
                return false;

            // direkte Rückkopplung (stand jetzt) möglich aber unerwünscht.
            if (fromEp.ViewModel == toEp.ViewModel) 
                return false;

            int outCount = 0;

            // grundsätzliche Prüfung, 1 Eingang darf nicht mit mehr als 1 Ausgang verbunden sein:
            foreach (GateConnectionViewModel gcvm in _GateConnections)
            {
                if (gcvm.Identical(toEp)) // Multibelegung Eingang - damit wird exakte Konfiguration ebenfalls unterbunden
                    return false;

                // zählen wie oft Ausgang bereits verwendet, ist begrenzt:
                if (gcvm.Identical(fromEp))
                {
                    outCount++;

                    if (outCount >= cMaxOutputUsage)
                        return false;
                }
            }

            // wenn 2 netze (multipolyline) miteinander verbunden werden, müssen diese zu einem netz zusammengefasst werden.
            // das Darf natürlich nur dann gehen, wenn nur ein Ausgang verbunden ist. Alternativ komplett blockieren,
            // sodass netze nur mit ein/ausgängen verbunden werden können.


            // Dnn sollte (vorerst) alles so passen, nun die Models und Darstellung erzeugen.
            GateConnectionModel gcm = new GateConnectionModel();

            gcm.FromGateId = fromEp.ViewModel.Model.Id;
            gcm.FromOutputIndex = fromEp.Index;
            gcm.ToGateId = toEp.ViewModel.Model.Id;
            gcm.ToInputIndex = toEp.Index;

            GateConnectionViewModel cvm = new GateConnectionViewModel(gcm, fromEp.ViewModel, toEp.ViewModel);

            Polyline connLine = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                DataContext = cvm
            };

            connLine.SetBinding(Polyline.PointsProperty, new Binding("Points")
            {
                Source = cvm,
                Converter = new PointsConverter(),
                Mode = BindingMode.OneWay
            });

            _Canvas.Children.Add(connLine);
            _Lines.Add(connLine);
            _GateConnections.Add(cvm);

            ConnectionAdded?.Invoke(gcm);

            return true;
        }

    }
}
