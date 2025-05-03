using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using CardAccessControl.Models;
using CardAccessControl.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using CardAccessControl.Services;

namespace CardAccessControl.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MqttService _mqtt;

        private ObservableCollection<AccessTime> _accessTimes = new();
        public ObservableCollection<AccessTime> AccessTimes
        {
            get => _accessTimes;
            set { _accessTimes = value; OnPropertyChanged(); }
        }

        public MainViewModel(MqttService mqtt)
        {
            _mqtt = mqtt;
            _mqtt.AccessTimeLogged += Mqtt_AccessTimeLogged;   // 🔔 subscribe

            _ = LoadAccessTimesAsync();                        // initial fill
        }

        private async Task LoadAccessTimesAsync()
        {
            await using var context = new AccessControlContext();
            var items = await context.AccessTimes
                                      .OrderByDescending(at => at.Time)
                                      .ToListAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                AccessTimes.Clear();
                foreach (var item in items)
                    AccessTimes.Add(item);
            });
        }
        private void Mqtt_AccessTimeLogged(object? s, AccessTime at)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                AccessTimes.Insert(0, at);
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
