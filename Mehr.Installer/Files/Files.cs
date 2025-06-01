using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using static System.Environment;
using System.Net;
using Mehr.Installer.Properties;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;


namespace Mehr.Installer
{
    public class Files
    {
        public readonly string FontFileDownloadAddress;
        public readonly string MehrFileDownloadAddress;
        public readonly string Sql32BitDownloadAddress;
        public readonly string Sql64BitDownloadAddress;
        public readonly string FontFilePath;
        public readonly string MehrFilePath;
        public readonly string Sql32BitPath;
        public readonly string Sql64BitPath;
        public readonly string FontFileZipPath;
        public readonly string MehrFileZipPath;
        public readonly string Sql32BitZipPath;
        public readonly string Sql64BitZipPath;
        public readonly string ApplicationPath;
        public delegate void CopyProgressEvent(int progress);
        public event CopyProgressEvent CopyProgressEventHnadler;
        public delegate void ExtractProgressEvent(int progress);
        public event ExtractProgressEvent ExtractProgressEventHandler;

        public Files()
        {
            ApplicationPath = Application.StartupPath + "\\";
            string BaseAddress = GetBaseDownloadAddress();
            FontFileDownloadAddress = BaseAddress + "FarsFont.zip";
            MehrFileDownloadAddress = BaseAddress + "MehrFile.zip";
            Sql64BitDownloadAddress = BaseAddress + "Sqlexpress.zip";
            Sql32BitDownloadAddress = BaseAddress + "Sqlexpressx86.zip";
            FontFilePath = ApplicationPath + "FarsFont";
            MehrFilePath = ApplicationPath + "MehrFile";
            Sql64BitPath = ApplicationPath + "Sqlexpress";
            Sql32BitPath = ApplicationPath + "Sqlexpressx86";
            FontFileZipPath = ApplicationPath + "FarsFont.zip";
            MehrFileZipPath = ApplicationPath + "MehrFile.zip";
            Sql64BitZipPath = ApplicationPath + "Sqlexpress.zip";
            Sql32BitZipPath = ApplicationPath + "Sqlexpressx86.zip";
        }

        public string GetBaseDownloadAddress()
        {
            CreateWebApiAddressFile();
            string address = File.ReadAllText(ApplicationPath + "WebApiAddress.inf");
            return address + "/Files/InstallFile/";
        }
        public void CreateWebApiAddressFile()
        {
            if (!File.Exists(ApplicationPath + "WebApiAddress.inf"))
            {
                FileStream LanFile = File.Create(ApplicationPath + "WebApiAddress.inf");
                LanFile.Dispose();
                File.WriteAllText(ApplicationPath + "WebApiAddress.inf", "http://mehrbastan.net/");
            }
        }
        #region Check Exists Zip File
        public bool IsExistsFarsFontZipFile()
        {
            if (File.Exists(FontFileZipPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsExistsMehrZipFile()
        {
            if (File.Exists(MehrFileZipPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsExistsSql64BitZipFile()
        {
            if (File.Exists(Sql64BitZipPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsExistsSql32BitZipFile()
        {
            if (File.Exists(Sql32BitZipPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region Unzip File
        public void UnzipFarsFont()
        {
            try
            {
                if (!Directory.Exists(FontFilePath))
                {
                    Directory.CreateDirectory(FontFilePath);
                    if (File.Exists(FontFileZipPath))
                    {
                        ZipFile.ExtractToDirectory(FontFileZipPath, FontFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.UnzipFontsFileError, ex);
            }
        }
        public void UnzipMehrFile(ref long fileSize, ref long extractedSizeTotal, ref long compressedSize, ref string compressedFileName)
        {
            try
            {
                if (Directory.Exists(MehrFilePath))
                {
                    Directory.Delete(MehrFilePath, true);
                }
                Directory.CreateDirectory(MehrFilePath);
                if (File.Exists(MehrFileZipPath))
                {
                    // ZipFile.ExtractToDirectory(MehrFileZipPath, MehrFilePath);
                    fileSize = 0;
                    extractedSizeTotal = 0;
                    compressedSize = 0;
                    compressedFileName = "";
                    ExtractByProgress(MehrFileZipPath, MehrFilePath,ref fileSize,ref extractedSizeTotal,ref compressedSize,ref compressedFileName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.UnzipMehrFileError, ex);
            }

        }
        public void UnzipSql64File(ref long fileSize, ref long extractedSizeTotal, ref long compressedSize, ref string compressedFileName)
        {
            try
            {
                if (Directory.Exists(Sql64BitPath))
                {
                    Directory.Delete(Sql64BitPath, true);
                }
                if (File.Exists(Sql64BitZipPath))
                {
                    //ZipFile.ExtractToDirectory(Sql64BitZipPath, Sql64BitPath);
                    fileSize = 0;
                    extractedSizeTotal = 0;
                    compressedSize = 0;
                    compressedFileName = "";
                    ExtractByProgress(Sql64BitZipPath, Sql64BitPath, ref fileSize, ref extractedSizeTotal, ref compressedSize, ref compressedFileName);
                }
        
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.UnzipSql64FileError, ex);
            }

        }
        public void UnzipSql32File(ref long fileSize, ref long extractedSizeTotal, ref long compressedSize, ref string compressedFileName)
        {
            try
            {
                if (Directory.Exists(Sql32BitPath))
                {
                    Directory.Delete(Sql32BitPath);
                }
                if (File.Exists(Sql32BitZipPath))
                {
                    //ZipFile.ExtractToDirectory(Sql32BitZipPath, Sql32BitPath);
                    fileSize = 0;
                    extractedSizeTotal = 0;
                    compressedSize = 0;
                    compressedFileName = "";
                    ExtractByProgress(Sql32BitZipPath, Sql32BitPath, ref fileSize, ref extractedSizeTotal, ref compressedSize, ref compressedFileName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.UnzipSql64FileError, ex);
            }

        }

        public void ExtractByProgress(string fileName, string extractPath,
            ref long fileSize,ref long extractedSizeTotal,ref long compressedSize, ref string compressedFileName)
        {
            try
            {

                //get the size of the zip file
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                fileSize = fileInfo.Length;
                using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(fileName))
                {
                    //reset the bytes total extracted to 0
                    extractedSizeTotal = 0;
                    int fileAmount = zipFile.Count;
                    int fileIndex = 0;
                    zipFile.ExtractProgress +=(s,e) =>{
                        if (e.TotalBytesToTransfer > 0)
                        {
                            long percent = e.BytesTransferred * int.MaxValue / e.TotalBytesToTransfer;
                            //Console.WriteLine("Indivual: " + percent);
                            ExtractProgressEventHandler((int)percent);
                        }
                    };

                    foreach (Ionic.Zip.ZipEntry ZipEntry in zipFile)
                    {
                        fileIndex++;
                        compressedFileName = "(" + fileIndex.ToString() + "/" + fileAmount + "): " + ZipEntry.FileName;
                        //get the size of a single compressed file
                        compressedSize = ZipEntry.CompressedSize;
                        ZipEntry.Extract(extractPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                        //calculate the bytes total extracted
                        extractedSizeTotal += compressedSize;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int GetFileSize(string path)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
            return Convert.ToInt32(fileInfo.Length);
        }
        #endregion

        #region InstallFont
        public void InstallFont()
        {
            PrivateFontCollection fontCol = new PrivateFontCollection();
            string[] fonts = Directory.GetFiles(FontFilePath);

            foreach (string item in fonts)
            {
                fontCol.AddFontFile(item);
                var res = AddFontResource(item);
                //File.Copy(item,Path.Combine(Environment.GetFolderPath(SpecialFolder.Windows),"Fonts",Path.GetFileName(item)),true);
            }
        }
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]string lpFileName);

        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW" +
            "", SetLastError = true)]
        public static extern int RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)]string lpFileName);
        #endregion

        #region InstallMehr
        public void InstallMehr(string destinationPath)
        {
            try
            {
                int count = 0;
                CopyDirectory(MehrFilePath, destinationPath, true, ref count);
                DirectoryInfo dInfo = new DirectoryInfo(destinationPath+"\\Data");
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.ErrorWhileInstallMehr, ex);
            }
        }
        public void CopyDirectory(string sourceDir, string destinationDir, bool recursive, ref int count)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
                count += 1;
                CopyProgressEventHnadler(count);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true, ref count);
                }
            }
        }

        public int GetCountFile(string directoryPath)
        {
            int count = 0;
            CountOfFilesDirectory(directoryPath, true, ref count);
            return count;
        }
        static void CountOfFilesDirectory(string sourceDir, bool recursive, ref int count)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();


            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                count += 1;
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    CountOfFilesDirectory(subDir.FullName, true, ref count);

                }
            }
        }
        #endregion

        #region Save Connection Setting
        public static bool SaveConnectionSetting(string connstrdisplay, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string[] AllLines = File.ReadAllLines(path);
                    var Copy = new string[AllLines.Length];
                    var NoOfRemoved = default(int);
                    for (int I = 0, loopTo = AllLines.Length - 1; I <= loopTo; I++)
                    {
                        if (!AllLines[I].ToLower().StartsWith("0*/cnnstring"))
                        {
                            Copy[I - NoOfRemoved] = AllLines[I];
                        }
                        else
                        {
                            NoOfRemoved += 1;
                        }
                    }
                    Array.Resize(ref Copy, Copy.Length - NoOfRemoved);
                    int Index = Copy.Length;

                    List<string> item = new List<string>();
                    item.Add(connstrdisplay);
                    foreach (string S in AllLines)
                    {
                        string SS = S.ToLower();
                        if (SS.StartsWith("0*/cnnstring"))
                        {
                            if (SS.Remove(0, "0*/cnnstring00/*".Length) != connstrdisplay)
                            {
                                item.Add(SS.Remove(0, "0*/cnnstring00/*".Length));
                            }
                        }
                    }
                    Array.Resize(ref Copy, Copy.Length + item.Count);
                    for (int I = 0, loopTo1 = item.Count - 1; I <= loopTo1; I++)
                    {
                        Copy[Index] = "0*/cnnstring" + I.ToString("00") + "/*" + item[I];
                        Index += 1;
                    }
                    File.WriteAllLines(path, Copy);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
     
    }
}
