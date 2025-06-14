using SmaHo_C_IDE.EventHandler;
using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        private Point _DragOffset;

        public EditState? EditState { get; set; }

        public LogicGateBaseViewModel ViewModel { get; }

        public event GateDeletionRequestedEventHandler DeletionRequested;

        public LogicGateBaseControl(LogicGateBaseViewModel model, int inCount, int outCount)
        {

            // TODO Maushändigkeit anpassen!
            this.MouseLeftButtonDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseLeftButtonUp += OnMouseUp;
            ViewModel = model;
            this.DataContext = model;
            model.InitializePoints(inCount, outCount);

        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditState == null || !EditState.IsActivated)
                return;

            if (EditState.EditMode == Services.EditMode.Select)
            {
                IsDragging = true;
                _DragOffset = e.GetPosition(this);

                this.CaptureMouse();
            }

            if (EditState.EditMode == Services.EditMode.Remove)
            {
                DeletionRequested?.Invoke(ViewModel);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (EditState == null || !EditState.IsActivated)
                return;

            if (EditState.EditMode != Services.EditMode.Select)
                return;

            if (IsDragging)
            {
                var parent = VisualTreeHelper.GetParent(this) as Canvas;

                if (parent == null) return;

                var position = e.GetPosition(parent);

                double x = position.X - _DragOffset.X;
                double y = position.Y - _DragOffset.Y;

                SetPosition(x, y);
            }
        }

        public void SetPosition(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);

            ViewModel.CurrentPosition = new Point(x, y);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (EditState == null || !EditState.IsActivated)
                return;

            if (EditState.EditMode == Services.EditMode.Select)
            {
                if (IsDragging)
                {
                    IsDragging = false;
                    this.ReleaseMouseCapture();
                }
            }
        }


    }
}
