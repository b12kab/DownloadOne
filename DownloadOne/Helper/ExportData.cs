using System;
using Xamarin.Forms;

namespace DownloadOne.Helper
{
    public class ExportData
    {
        public bool CreateExportFile(string text)
        {
            return DependencyService.Get<IExport>().ExportFile(text, "testing_file.txt");
        }
    }
}
