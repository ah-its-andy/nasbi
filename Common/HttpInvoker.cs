using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NasBI.Common
{
    public static class HttpInvoker
    {
        private static ReaderWriterLock ReaderWriterLock = new ReaderWriterLock();
        public static HttpActionList HttpLogs { get; } = new HttpActionList();
        public static async Task<HttpResponseMessage> InvokeAsync(string identifier, HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage.RequestUri == null)
            {
                return null;
            }
            httpRequestMessage.Options.Set(new HttpRequestOptionsKey<TimeSpan>("RequestTimeout"), TimeSpan.FromSeconds(15));
            var client = new HttpClient();
            await AddHttpLogs(new HttpActionModel
            {
                Identifier = identifier,
                Path = httpRequestMessage.RequestUri.ToString(),
                Status = "pending",
                Duration = ""
            });
            var sw = new Stopwatch();
            sw.Start();
            var resp = await client.SendAsync(httpRequestMessage);
            sw.Stop();
            await ModifyHttpLog(new HttpActionModel
            {
                Identifier = identifier,
                Status = $"({(int)resp.StatusCode}){resp.StatusCode.ToString()}",
                Duration = $"{sw.Elapsed.TotalMilliseconds}ms"
            });
            return resp;
        }

        private static async Task AddHttpLogs(HttpActionModel httpAction)
        {
            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    HttpLogs.Add(httpAction);
                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async  Task ModifyHttpLog(HttpActionModel httpAction)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (var item in HttpLogs)
                {
                    if (httpAction.Identifier == item?.Identifier)
                    {
                        item.Status = httpAction.Status;
                        item.Duration = httpAction.Duration ?? "";
                    }
                }
            });
            
        }
    }

    public class HttpActionModel : INotifyPropertyChanged
    {
        private string status;
        private string duration;


        public string Identifier { get; set; }
        public string Path { get; set; }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }
        public string Duration
        {
            get => duration;
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Duration"));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class HttpActionList : BlockingCollection<HttpActionModel>, INotifyCollectionChanged
    {
        public new void Add(HttpActionModel item)
        {
            ((BlockingCollection<HttpActionModel>)this).Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<HttpActionModel> { item }));
        }

        public object? this[int index] { get => this[index]; set => this[index] = value; }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
    }

}
