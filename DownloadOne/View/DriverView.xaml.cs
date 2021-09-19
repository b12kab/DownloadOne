using DownloadOne.ViewModel;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DownloadOne.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriverView : ContentPage
    {
        public DriverView()
        {
            InitializeComponent();
            this.BindingContext = new DownloadViewModel(Navigation);
        }
    }
}
