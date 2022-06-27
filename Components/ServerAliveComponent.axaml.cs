using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using NasBI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace NasBI.Components
{
    public partial class ServerAliveComponent : UserControl
    {
        public ServerAliveComponent()
        {
            InitializeComponent();
            DataSource = new DataContextModel();
            DataContext = DataSource;
        }

        public DataContextModel DataSource { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DataSource.ServerAlives.Add(DataSource.UnraidState);
            DataSource.ServerAlives.Add(DataSource.TruenasState);
            Task.Run(async () =>
            {
                for (; ; )
                {
                    await FetchUnraidAsync();
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            });
            Task.Run(async () =>
            {
                for (; ; )
                {
                    await FetchTruenasAsync();
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            });
        }

        private async Task FetchUnraidAsync()
        {
            var config = Config.LoadConfig();

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(config.UnraidServerHealthCheckUrl);
            
            var resp = await HttpInvoker.InvokeAsync(Guid.NewGuid().ToString(), request);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (resp.IsSuccessStatusCode)
                {
                    DataSource.UnraidState.State = "online";
                }
                else
                {
                    DataSource.UnraidState.State = "offline";
                }
            });
           
        }

        private async Task FetchTruenasAsync()
        {
            var config = Config.LoadConfig();

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(config.TruenasServerHealthCheckUrl);

            var resp = await HttpInvoker.InvokeAsync(Guid.NewGuid().ToString(), request);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (resp.IsSuccessStatusCode)
                {
                    DataSource.TruenasState.State = "online";
                }
                else
                {
                    DataSource.TruenasState.State = "offline";
                }
            });
        }


        public class DataContextModel
        {
            public ObservableCollection<ServiceAliveModel> ServerAlives { get; private set; } = new ObservableCollection<ServiceAliveModel>();
            public ServiceAliveModel UnraidState { get; } = new ServiceAliveModel { ServerName = "unraid", State = "unknown" };
            public ServiceAliveModel TruenasState { get; } = new ServiceAliveModel { ServerName = "truenas", State = "unknown" };

        }
    }

    public class ServiceAliveModel : INotifyPropertyChanged
    {
        private string state;
        public string ServerName { get; set; }
        public string State
        {
            get => state;
            set
            {
                state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }


}
