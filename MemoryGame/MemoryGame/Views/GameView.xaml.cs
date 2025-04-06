using MemoryGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGame.Views
{

    public partial class GameView : Window
    {
        public GameView()
        {
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
           
            if (this.DataContext is GameViewModel vm)
            {
                if (vm.IsForcedQuit)
                {
                    base.OnClosed(e);
                    var logwind = new LoginView();
                    logwind.Show();
                }
            }
        }
    }
}
