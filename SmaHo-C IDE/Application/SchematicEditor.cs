using SmaHo_C_IDE.Helper;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Services;
using SmaHo_C_IDE.ViewModels;
using SmaHo_C_IDE.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SmaHo_C_IDE.Application
{

    // enthält die eigentliche Logik des Editors
    class SchematicEditor
    {
        GateFactory _GateFactory;
        IdManager _IdManager;

        EditorPage? _CurrentPage;
        List<EditorPage> _Pages;

        List<LogicGateBaseModel> _LogicGates;

        TabControl _TabControl;

        int _PageCounter = 0;

        EditMode _CurrentMode;
        GateType _CurrentNewGateType;

        public SchematicEditor(TabControl tabControl)
        {
            _IdManager = new IdManager();
            _GateFactory = new GateFactory(_IdManager);
            _Pages = new List<EditorPage>();

            _LogicGates = new List<LogicGateBaseModel>();

            _TabControl = tabControl;
            _CurrentMode = EditMode.None;

            AddNewPage();

            _TabControl.SelectionChanged += PageTabSelectChanged;
        }

        public void AddGateMode(GateType gateType)
        {
            _CurrentMode = EditMode.AddGate;
            _CurrentNewGateType = gateType;
        }

        public void SetDragDrop()
        {
            _CurrentMode = EditMode.Select;
        }

        private void AddNewPage()
        {
            TabItem ti = new TabItem();
            ScrollViewer sv = new ScrollViewer();

            Canvas c = new Canvas();
            // Todo Translations?
            ti.Header = "Seite " + (_Pages.Count + 1);
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            // Todo: später konfigurierbar machen.
            c.Width = 1600;
            c.Height = 1130;
            c.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xef, 0xef, 0xef));

            ti.Content = sv;
            sv.Content = c;
            _CurrentPage = new EditorPage(c);
            _CurrentPage.Title = ti.Name;
            _CurrentPage.Id = _PageCounter++;
            _CurrentPage.EditState.IsActivated = true;
            _CurrentPage.MouseDown += EditorPageOnMouseDown;

            _Pages.Add(_CurrentPage);

            if (_TabControl.Items.Count > 0)
            {
                _TabControl.Items.RemoveAt(_TabControl.Items.Count - 1);
            }

            _TabControl.Items.Add(ti);
            _TabControl.SelectedIndex = _TabControl.Items.Count - 1;

            ti = new TabItem();
            ti.Header = "+";
            _TabControl.Items.Add(ti);
        }

        private void EditorPageOnMouseDown(EditorPage page, MouseButtonEventArgs e)
        {
            // dieses Event betrifft das Canvas, nicht das Gesamte EditorPage
            Canvas c = page.Canvas;

            var position = e.GetPosition(c);

            if (_CurrentMode == EditMode.AddGate)
            {
                (LogicGateBaseViewModel, LogicGateBaseControl) gate = _GateFactory.CreateGate(_CurrentNewGateType);

                c.Children.Add(gate.Item2);
                gate.Item2.EditState = page.EditState;
                

                Canvas.SetLeft(gate.Item2, position.X - (gate.Item2.Width / 2));
                Canvas.SetTop(gate.Item2, position.Y - (gate.Item2.Height / 2));
            }
            else { }



        }

        private void PageTabSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl s = (TabControl)sender;

            if (s != null && s.SelectedIndex > 0 && s.SelectedIndex < s.Items.Count)
            {
                TabItem? ti = s.Items[s.SelectedIndex] as TabItem;

                if (ti != null)
                {
                    if (_CurrentPage != null)
                    {
                        _CurrentPage.EditState.IsActivated = false;
                    }

                    if ((string)ti.Header == "+")
                    {
                        AddNewPage();
                    }
                    else if (_CurrentPage != null)
                    {
                        _CurrentPage = _Pages[s.SelectedIndex];
                        _CurrentPage.EditState.IsActivated = true;
                    }
                }
            }
        }
    }
}
