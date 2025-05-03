using System.Windows;
using System.Windows.Controls;

namespace AccessControlSystem.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        public void Navigate(Page page) => MainFrame.Navigate(page);
    }
}
