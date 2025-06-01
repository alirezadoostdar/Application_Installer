using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace CustomActionOpenFolder
{
    [RunInstaller(true)]
    public class OpenFolderAction : Installer
    {
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                // Get the installation directory (TARGETDIR) passed by the installer
                string installDir = Context.Parameters["targetdir"]?.TrimEnd('\\');

                if (!string.IsNullOrEmpty(installDir) && Directory.Exists(installDir))
                {
                    // Start explorer.exe to open the installation folder
                    Process.Start("explorer.exe", installDir);
                }
                else
                {
                    // Log error for debugging
                    File.WriteAllText("C:\\Temp\\CustomActionLog.txt",
                        $"Installation directory not found or invalid: {installDir}");
                }
            }
            catch (Exception ex)
            {
                // Log error for debugging
                File.WriteAllText("C:\\Temp\\CustomActionLog.txt",
                    $"Error opening folder: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }
    }
}
