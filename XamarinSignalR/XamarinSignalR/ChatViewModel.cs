using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinSignalR
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        HubConnection hubConnection;

        public string UserName { get; set; }
        public string Message { get; set; }

        public ObservableCollection<MessageData> MessageData { get; }

        bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }

        public Command SendMessageCommand { get; }

        public ChatViewModel()
        {
            hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:5153/chat")
                .Build();

            MessageData = new ObservableCollection<MessageData>();

            IsConnected = false;
            IsBusy = false;

            SendMessageCommand = new Command(async () => await SendMessage(), () => IsConnected);

            hubConnection.Closed += async (error) =>
            {
                SendLocalMessage(String.Empty, "Подключение закрыто...");
                IsConnected = false;
                await Task.Delay(5000);
                await Connect();
            };

            hubConnection.On<string, string>("Receive", (user, message) =>
            {
                SendLocalMessage(user, message);
            });
        }

        public async Task Connect()
        {
            if (IsConnected)
                return;
            try
            {
                await hubConnection.StartAsync();
                SendLocalMessage(String.Empty, "Вы вошли в чат...");

                IsConnected = true;
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка подключения: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            if (!IsConnected)
                return;

            await hubConnection.StopAsync();
            IsConnected = false;
            SendLocalMessage(String.Empty, "Вы покинули чат...");
        }

        private async Task SendMessage()
        {
            try
            {
                IsBusy = true;
                await hubConnection.InvokeAsync("Send", UserName, Message);
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SendLocalMessage(string user, string message)
        {
            MessageData.Insert(0, new MessageData
            {
                Message = message,
                User = user
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
