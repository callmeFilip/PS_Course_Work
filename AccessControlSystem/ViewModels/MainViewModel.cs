using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using AccessControlSystem.Models;
using AccessControlSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using AccessControlSystem.Services;

namespace AccessControlSystem.ViewModels
{
    /// <summary>
    ///  Shows the last N access events and updates instantly when MqttService
    ///  reports a new swipe.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int _pageSize = 100;
        private readonly IUnitOfWork _uow;

        public ObservableCollection<AccessTime> AccessTimes { get; } = new();


        public MainViewModel(IUnitOfWork uow, MqttService mqtt)
        {
            _uow = uow;

            mqtt.AccessTimeLogged += Mqtt_AccessTimeLogged;   // Subscribe

            _ = LoadAccessTimesAsync();                        // initial fill
        }

        private async Task LoadAccessTimesAsync()
        {
            try
            {
                var latest = await _uow.AccessTimes.GetLatestAsync(_pageSize);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AccessTimes.Clear();
                    foreach (var a in latest) AccessTimes.Add(a);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadAccessTimesAsync failed: {ex}");
            }
        }
        private void Mqtt_AccessTimeLogged(object? s, AccessTime at)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AccessTimes.Insert(0, at);
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
