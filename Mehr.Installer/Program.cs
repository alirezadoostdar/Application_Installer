using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mehr.Installer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            changecustomculture();
            Application.Run(new InstallWizard());
        }

        private static void changecustomculture()
        {
            string culturenamestr = checkcultureincommandline();
            var systemculture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            var currentculture = new CultureInfo(culturenamestr);
            currentculture.NumberFormat = systemculture.NumberFormat;
            currentculture.DateTimeFormat = systemculture.DateTimeFormat;
            System.Threading.Thread.CurrentThread.CurrentUICulture = currentculture;
           // ResourceManager resourceManager = new ResourceManager("Resource.en.resx",Mehr.Installer);
        }
        private static string checkcultureincommandline()
        {

            try
            {
                int Language = Convert.ToInt32(File.ReadAllText(Application.StartupPath + "\\Language.inf"));
                
                string Res = "fa";
                switch (Language)
                {
                    case 1:
                        {
                            Res = "fa";
                            break;
                        }
                    case 0:
                        {
                            Res = "en";
                            break;
                        }
                    case 2:
                        {
                            Res = "ar";
                            break;
                        }

                    default:
                        {
                            Res = "fa";
                            break;
                        }
                }
                return Res;
            }
            catch (Exception ex)
            {
                return "fa";
            }
        }
    }
}
