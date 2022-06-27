using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NasBI.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NasBI.Components
{
    public partial class HttpRecordComponent : UserControl
    {
        private DataContextModel DataSource { get; }
        public HttpRecordComponent()
        {
            InitializeComponent();
            DataSource = new DataContextModel();
            DataSource.HttpLogs = HttpInvoker.HttpLogs;
            DataContext = DataSource;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public class DataContextModel
        {
            public HttpActionList HttpLogs { get; set; } 
        }

    }
}
