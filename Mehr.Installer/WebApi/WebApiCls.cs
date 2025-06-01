using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace Mehr.Installer
{
    public class WebApiCls
    {
        public readonly string BaseUrl;
        public readonly string ApplicationPath;
        public WebApiCls()
        {
            ApplicationPath = Application.StartupPath + "\\";
            CreateWebApiAddressFile();
            // BaseUrl = "http://localhost:58893/";
            BaseUrl = File.ReadAllText(Path.Combine(Application.StartupPath, "WebApiAddress.inf"));
        }
        public void CreateWebApiAddressFile()
        {
            if (!File.Exists(ApplicationPath + "WebApiAddress.inf"))
            {
                FileStream LanFile = File.Create(ApplicationPath + "WebApiAddress.inf");
                LanFile.Dispose();
                File.WriteAllText(ApplicationPath + "WebApiAddress.inf", "https://mehraccounting.com/");
            }
        }

        public void SendBugReport(BugReportViewModel bugReport)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    var Data = client.PostAsJsonAsync<BugReportViewModel>($"BugReport/Create/", bugReport).Result;
                    //return JsonConvert.DeserializeObject<int>(Data);
                }
            }
            catch (Exception)
            {

            }
        }

        public void CreateInstallerLog(InstallerLogViewModel installerLog)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseUrl);
                    var Data = client.PostAsJsonAsync<InstallerLogViewModel>($"InstallerLog/Create/", installerLog).Result;

                    // var id = Data.Content.ReadAsAsync<int>();
                    // return JsonConvert.DeserializeObject<int>(Data);
                }
            }
            catch (Exception)
            {

            }
        }

    }
}
