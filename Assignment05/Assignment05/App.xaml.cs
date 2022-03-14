using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Assignment05
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Views.CountryView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
