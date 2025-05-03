using CardAccessControl.ViewModels;
using System.Windows;

namespace CardAccessControl.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mqtt = ((App)Application.Current).MqttService;   // grab the live service
            DataContext = new MainViewModel(mqtt);
        }
    }
}
