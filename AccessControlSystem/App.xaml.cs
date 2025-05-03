using System.Windows;
using CardAccessControl.Services;
using System.Threading.Tasks;
using CardAccessControl.Models;
using CardAccessControl.Data;

namespace CardAccessControl
{
    public partial class App : Application
    {
        public MqttService MqttService { get; private set; } = null!;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConsoleManager.AllocConsole();

            // Initialize and connect the MQTT service.
            MqttService = new MqttService();
            await MqttService.ConnectAsync();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            ConsoleManager.FreeConsole();
            base.OnExit(e);
        }
    }
}
