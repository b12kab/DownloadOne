using DownloadOne.Helper;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DownloadOne.ViewModel
{
    public class DownloadViewModel : INotifyPropertyChanged
    {
        public INavigation _navigation;
        public ICommand DownloadCommand { get; private set; }

        public DownloadViewModel(INavigation navigation)
        {
            _navigation = navigation;

            DownloadCommand = new Command(async () => await ExportData());
        }

        private static string _fileContents;
        private ExportData exportInfo;

        async Task ExportData()
        {
            if (_fileContents == null)
            {
                Contents = "default text";
            }

            bool exportData = await Application.Current.MainPage.DisplayAlert("Export Data", "Do you want to export the data?", "OK", "Cancel");
            if (exportData)
            {
                bool permissionGranted = await CheckPermission();
                if (!permissionGranted)
                {
                    await Application.Current.MainPage.DisplayAlert("Export Data", "You didn't give permission. If you wish to give permission, please agree to the permission", "OK");
                }

                if (exportInfo == null)
                {
                    exportInfo = new ExportData();
                }

                bool worked = exportInfo.CreateExportFile(Contents);

                if (worked)
                {
                    await Application.Current.MainPage.DisplayAlert("Export Data", "Export worked", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Export Data", "Export Failed", "OK");
                }
            }
        }

        private async Task<bool> CheckPermission()
        {
            bool permissionNeeded = DependencyService.Get<IExport>().IsPermissionNeeded();

            if (!permissionNeeded)
            {
                return true;
            }

            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status == PermissionStatus.Granted)
            {
                return true;
            }

            if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                await Application.Current.MainPage.DisplayAlert("Export Data", "Without this permission, we can't export the file", "OK");
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();


            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        public string Contents
        {
            get => _fileContents;
            set
            {
                _fileContents = value;
                NotifyPropertyChanged("Contents");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
