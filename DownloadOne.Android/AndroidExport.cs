using Android.Annotation;
using Android.Content;
using DownloadOne.Helper;
using Plugin.CurrentActivity;
using System;
using System.IO;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(DownloadOne.Droid.AndroidExport))]
namespace DownloadOne.Droid
{
    public class AndroidExport : IExport
    {

        /// <summary>
        /// After version 10 (API 29) cannot write directly to Downloads directory, so permissions not needed.
        /// Before that, permission is needed.
        /// </summary>
        /// <returns></returns>
        public bool IsPermissionNeeded()
        {
            Version version = DeviceInfo.Version;

            if (version.Major < 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Write file out to app directory.
        /// </summary>
        /// <param name="contents">File content to write</param>
        /// <param name="filename">Filename to export content into</param>
        /// <returns>Worked</returns>
        public bool ExportFile(string contents, string filename)
        {

            Version version = DeviceInfo.Version;

            // After version 10 (API 29) cannot write directly to Downloads directory
            // and must do it via the Mediastore API due to security changes
            if (version.Major < 10)
            {
                return WriteAPI28AndBelow(contents, filename);
            }
            else
            {
                return WriteAPI29AndAbove(contents, filename);
            }
        }

        [TargetApi(Value = 28)]
        private bool WriteAPI28AndBelow(string contents, string filename)
        {
            string directory;

#pragma warning disable CS0618 // Type or member is obsolete
            directory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
#pragma warning restore CS0618 // Type or member is obsolete

            string filespec = Path.Combine(directory, filename);

            FileInfo newFile = new FileInfo(filespec);
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook

                    System.Diagnostics.Debug.WriteLine("existing file deleted: " + filespec);
                    System.Diagnostics.Debug.Flush();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to delete existing filespec: " + filename);
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

        private bool WriteAPI29AndAbove(string contents, string filename)
        {
            string fileNameWithoutExt = Path.ChangeExtension(filename, null);

            int fileSize = (int)contents.Length;

            ContentValues values = new ContentValues();
            ContentResolver contentResolver = CrossCurrentActivity.Current.AppContext.ContentResolver;

            values.Put(Android.Provider.MediaStore.IMediaColumns.Title, filename);
            values.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "text/plain");
            values.Put(Android.Provider.MediaStore.IMediaColumns.Size, fileSize);
            values.Put(Android.Provider.MediaStore.Downloads.InterfaceConsts.DisplayName, fileNameWithoutExt);

            Android.Net.Uri newUri;
            System.IO.Stream saveStream;

            try
            {
                newUri = contentResolver.Insert(Android.Provider.MediaStore.Downloads.ExternalContentUri, values);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to get data back from content resolver. Filename: " + filename);
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }

            try
            {
                saveStream = contentResolver.OpenOutputStream(newUri);

                using (StreamWriter writer = new(saveStream))
                {
                    writer.WriteAsync(contents);
                    writer.Close();
                    writer.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed file write: " + filename);
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.Flush();
                return false;
            }

            return true;
        }
    }
}
