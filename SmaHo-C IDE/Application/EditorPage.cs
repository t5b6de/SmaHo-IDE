using SmaHo_C_IDE.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmaHo_C_IDE.Application
{
    class EditorPage
    {
        public event Action<EditorPage, MouseButtonEventArgs> MouseDown;

        public int Id {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Canvas Canvas { get; }

        public EditState EditState { get; }

        public EditorPage(Canvas c) { 
            Canvas = c;
            EditState = new EditState();

            c.MouseDown += CanvasOnMouseDown;

        }

        private void CanvasOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!EditState.IsActivated)
                return;

            MouseDown?.Invoke(this, e);
        }
    }
}
