using SmaHo_C_IDE.Application;
using SmaHo_C_IDE.Models;
using SmaHo_C_IDE.Services;
using SmaHo_C_IDE.Views.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmaHo_C_IDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SchematicEditor _Editor;

        private void StandardGatterButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var button = sender as ToggleButton;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                //button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.;
                button.ContextMenu.IsOpen = true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _Editor = new SchematicEditor(PagesTabControl);
            this.DataContext = _Editor;
        }

        private void AddGate(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is GateType gateType)
            {
                _Editor.AddGateType(gateType);
                _Editor.CurrentEditMode = EditMode.AddGate;
            }
        }

    }
}