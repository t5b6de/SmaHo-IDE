using SmaHo_C_IDE.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SmaHo_C_IDE.Views.Controls
{
    public class LogicGateBaseControl : UserControl
    {
        public bool IsDragging { get; private set; }

        protected Point[] _InputAnchors, _OutputAnchors;

        private Point _DragOffset;

        public EditState? EditState { get; set; }

        public LogicGateBaseControl(int inCount, int outCount)
        {
            this.Loaded += LogicGateBase_Loaded;
            this.MouseLeftButtonDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseLeftButtonUp += OnMouseUp;
            _InputAnchors = new Point[inCount];
            _OutputAnchors = new Point[outCount];
        }

        protected void AddAnchorFromLine(Line line, int index, bool isInput)
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

        private void LogicGateBase_Loaded(object sender, RoutedEventArgs e)
        {
            // Init-Logik (z. B. Verbindungspunkte setzen)
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditState == null || !EditState.IsActivated)
                return;

            IsDragging = true;
            _DragOffset = e.GetPosition(this);

            this.CaptureMouse();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (EditState == null || !EditState.IsActivated)
                return;

            if (IsDragging)
            {
                var parent = VisualTreeHelper.GetParent(this) as Canvas;
                if (parent == null) return;

                var position = e.GetPosition(parent);

                Canvas.SetLeft(this, position.X - _DragOffset.X);
                Canvas.SetTop(this, position.Y - _DragOffset.Y);

                /*Canvas.SetLeft(this, position.X - this.ActualWidth / 2);
                Canvas.SetTop(this, position.Y - this.ActualHeight / 2);*/

            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (_EditState == null || !EditState.IsActivated)
            //    return;

            IsDragging = false;
            this.ReleaseMouseCapture();
        }


    }
}
