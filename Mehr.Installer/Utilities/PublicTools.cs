using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Device.Location;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.IO;
using System;
using System.Reflection;
using IWshRuntimeLibrary;
using Mehr.Installer.Properties;

namespace Mehr.Installer
{
    public class PublicTools
    {
        public static void SendBugReport(Exception exc, MethodBase methodName, string dbVersion, string additionMsg = "")
        {

            WebApiCls webApi = new WebApiCls();
            BugReportViewModel bug = new BugReportViewModel();
            bug.Os_Version = Environment.OSVersion.Version.ToString();
            bug.App_Version = Application.ProductVersion;
            bug.DB_Version = dbVersion;
            bug.Exe_Path = Application.ExecutablePath;
            bug.Is_Client = false;// AppVar.ISClient.ToString
            bug.Server_Name = "";//AppVar.ServerName;
            bug.ConnectionString = "";// AppVar.DefaultConnection.ConnectionString.Replace("bastan", "mehr").Replace("bastanuser", "mehr").Replace((7000 + 943).ToString, "2000")
            bug.UserShamsiDate = ShamsiDate.ToShamsi(DateTime.Today);
            bug.UserMiladiDate = DateTime.Today;
            bug.HID = "";
            bug.Method_Name = methodName.ReflectedType.FullName + "." + methodName.Name;
            bug.ErrorMessage = exc.Message + "-" + "Additional Msg:" + additionMsg;
            if (exc.InnerException != null)
            {
                bug.InnerException = exc.InnerException.Message;
            }

            webApi.SendBugReport(bug);
        }


        public static void InstallerLog(string error, int bugReportId, bool isSuccess)
        {
            try
            {
                InstallerLogViewModel installerLog = new InstallerLogViewModel();
                try
                {
                    var geolocationInfo = GetIp_Info();
                    installerLog.Client_Ip = geolocationInfo.query;
                    installerLog.City = geolocationInfo.city;
                    installerLog.Country = geolocationInfo.country;
                    installerLog.CountryCode = geolocationInfo.countryCode;
                    installerLog.Isp = geolocationInfo.isp;
                    installerLog.Lat = geolocationInfo.lat.ToString();
                    installerLog.Lon = geolocationInfo.lon.ToString();
                    installerLog.Org = geolocationInfo.org;
                    installerLog.Region = geolocationInfo.region;
                    installerLog.RegionName = geolocationInfo.regionName;
                    installerLog.Timezone = geolocationInfo.timezone;
                }
                catch (Exception)
                {

                }
                try
                {
                    var loaction = GetLocalLatAndLong();
                    installerLog.Latitude = loaction.Latitude;
                    installerLog.Longitude = loaction.Longitude;
                }
                catch (Exception)
                {

                }
                installerLog.Error = error;
                installerLog.ErrorId = bugReportId;
                installerLog.IsSuccess = isSuccess;
                WebApiCls webApi = new WebApiCls();
                webApi.CreateInstallerLog(installerLog);
            }
            catch (Exception)
            {

            }
        }

        public static LocalLocation GetLocalLatAndLong()
        {
            LocalLocation location = new LocalLocation();
            try
            {
                GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
                GeoCoordinate coord;
                int i = 0;
                do
                {
                    try
                    {
                        watcher.TryStart(false, TimeSpan.FromMilliseconds(3000));
                        coord = watcher.Position.Location;
                        if (!double.IsNaN(coord.Latitude)) location.Latitude = Convert.ToDecimal(coord.Latitude);
                        if (!double.IsNaN(coord.Longitude)) location.Longitude = Convert.ToDecimal(coord.Longitude);
                    }
                    catch (Exception)
                    {
                        location.Latitude = 0;
                        location.Longitude = 0;
                    }
                    i += 1;
                } while (location.Latitude == 0 && i < 5);
                watcher.Stop();
                return location;
            }
            catch (Exception)
            {
                return location;
            }
        }

        public static LocationDetails_IpApi GetIp_Info()
        {
            LocationDetails_IpApi geolocationInfo = new LocationDetails_IpApi();
            try
            {
                string Ip = Get_IP();
                var Ip_Api_Url = $"http://ip-api.com/json/{Ip}";
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    // Pass API address to get the Geolocation details 
                    httpClient.BaseAddress = new Uri(Ip_Api_Url);
                    HttpResponseMessage httpResponse = httpClient.GetAsync(Ip_Api_Url).GetAwaiter().GetResult();
                    // If API is success and receive the response, then get the location details
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        geolocationInfo = httpResponse.Content.ReadAsAsync<LocationDetails_IpApi>().GetAwaiter().GetResult();
                    }

                }
                return geolocationInfo;
            }
            catch (Exception)
            {
                return geolocationInfo;
            }
        }

        public static string Get_IP()
        {
            try
            {
                string pubIp = new System.Net.WebClient().DownloadString("http://api.ipify.org");
                return pubIp;
            }
            catch (Exception)
            {
                return "";
            }
        }

        #region Create Desktop Shortcut
        public static void CreateShortcut(string path,string dbName)
        {
            try
            {
                object shortcutDesktop = (object)"DeskTop";
                WshShell shell = new WshShell();
                String strText = System.IO.Path.GetFileName(path);
                String strShortcutPath = (String)shell.SpecialFolders.Item(ref shortcutDesktop) + @"\" + string.Format(Resources.MehrTitle,dbName) + ".lnk";
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(strShortcutPath);
                shortcut.Hotkey = "ctrl+shift+m";
                shortcut.Description = "Exe Path: " + path + @"n\ Shortcut: ctrl+shift+m";
                shortcut.TargetPath = path;
                shortcut.Save();
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }
    public class LocationDetails_IpApi
    {
        public string query { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string isp { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string org { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string status { get; set; }
        public string timezone { get; set; }
        public string zip { get; set; }
    }

    public class LocalLocation
    {
        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
    }
}
