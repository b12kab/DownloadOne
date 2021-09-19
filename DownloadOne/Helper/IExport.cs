using System;
namespace DownloadOne.Helper
{
    public interface IExport
    {
        bool IsPermissionNeeded();

        bool ExportFile(string contents, string filename);
    }
}
