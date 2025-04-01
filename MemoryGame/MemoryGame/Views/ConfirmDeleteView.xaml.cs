using System.Windows;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class ConfirmDeleteView : Window
    {
        // Constructor care primește numele userului
        public ConfirmDeleteView(string userName)
        {
            InitializeComponent();
            this.DataContext = new ConfirmDeleteViewModel(userName, this);
        }
    }
}
