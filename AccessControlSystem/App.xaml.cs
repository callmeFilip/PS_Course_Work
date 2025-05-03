using System.Windows;
using AccessControlSystem.Services;
using AccessControlSystem.Data;
using AccessControlSystem.ViewModels;

namespace AccessControlSystem
{
    public partial class App : Application
    {
        public IUnitOfWork UnitOfWork { get; private set; } = null!;
        public MqttService MqttService { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConsoleManager.AllocConsole();
            
            /* Bootstrap data layer */
            UnitOfWork = new UnitOfWork(new AccessControlContext());

            
            /* Create live MQTT link that shares the same UoW */
            MqttService = new MqttService(UnitOfWork);
            await MqttService.ConnectAsync();

            /* Show UI */
            var mainWindow = new Views.MainWindow
            {
                DataContext = new MainViewModel(UnitOfWork, MqttService)
            };

            mainWindow.Show();
            mainWindow.Navigate(new Views.AccessLogPage(UnitOfWork, MqttService));
        }
        protected override void OnExit(ExitEventArgs e)
        {
            MqttService.DisposeAsync().AsTask().Wait();
            UnitOfWork.DisposeAsync().AsTask().Wait();
            ConsoleManager.FreeConsole();
            base.OnExit(e);
        }
    }
}
