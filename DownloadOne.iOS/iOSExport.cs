using DownloadOne.Helper;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(DownloadOne.iOS.iOSExport))]
namespace DownloadOne.iOS
{
    public class iOSExport : IExport
    {
        /// <summary>
        /// iOS will always return true for file write, as all files are local
        /// </summary>
        /// <returns>Permission needed? always true</returns>
        public bool IsPermissionNeeded()
        {
            return true;
        }

        /// <summary>
        /// Write file out to app directory.
        /// </summary>
        /// <param name="contents">File content to write</param>
        /// <param name="filename">Filename to export content into</param>
        /// <returns>Worked</returns>
        public bool ExportFile(string contents, string filename)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Documents folder  

            string filespec = Path.Combine(dir, filename);

            FileInfo newFile = new FileInfo(filespec);
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();

                    System.Diagnostics.Debug.WriteLine("existing file deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed file delete. Passed in filename: " + filespec + ". Full filespec: " + filespec);
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    System.Diagnostics.Debug.Flush();
                    return false;
                }
            }

            try
            {
                using (StreamWriter writer = new(filespec))
                {
                    writer.WriteAsync(contents);
                    writer.Close();
                    writer.DisposeAsync();
                }
                System.Diagnostics.Debug.WriteLine("File write succeeded. filename: " + filespec);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed file write. Passed in filename: " + filespec + ". Full filespec: " + filespec);
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }

            return true;
        }
    }
}
