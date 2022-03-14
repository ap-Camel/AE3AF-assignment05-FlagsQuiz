using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Assignment05.ViewModels;


namespace Assignment05.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryView : ContentPage
    {

        // the associated ViewModel
        CountryViewModel vm;

        public CountryView()
        {
            InitializeComponent();

            // initialize the ViewModel
            vm = new CountryViewModel();
            BindingContext = vm;

        }

        // this method executes when the page is displayed in the device
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // call the command to load the data
            await Task.Run(() => vm.LoadDataCommand.Execute(null));
        }

    }
}