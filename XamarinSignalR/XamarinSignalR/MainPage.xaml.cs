using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinSignalR
{
    public partial class MainPage : ContentPage
    {
        ChatViewModel chatViewModel;
        public MainPage()
        {
            InitializeComponent();
            chatViewModel = new ChatViewModel();
            BindingContext = chatViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await chatViewModel.Connect();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await chatViewModel.Disconnect();
        }
    }
}
