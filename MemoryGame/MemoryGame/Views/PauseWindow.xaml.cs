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
    public partial class PauseWindow : Window
    {
        public PauseWindow()
        {
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (this.DataContext is PauseViewModel vm)
            {
                if (vm.ResumeOnClose)
                {
                    vm.ContinueGame(null);
                }
            }
        }

    }

}

