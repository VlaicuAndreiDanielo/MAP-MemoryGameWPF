using System.Windows;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class ConfirmDeleteView : Window
    {
        public ConfirmDeleteView(string userName)
        {
            InitializeComponent();
            this.DataContext = new ConfirmDeleteViewModel(userName, this);
        }
    }
}
