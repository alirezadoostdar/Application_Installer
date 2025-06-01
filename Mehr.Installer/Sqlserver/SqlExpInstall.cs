using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    public class SqlExpInstall
    {
        #region Internal variables  

        //Variables for setup.exe command line  
        private string ACTION = "Install";
        public bool IACCEPTSQLSERVERLICENSETERMS = true;
        private string FEATURES = "SQLENGINE";
        public string SECURITYMODE = "SQL";
        public string SAPWD = "bastan.net.sqlserver";
        public string INSTANCENAME = "MEHRSQLSERVER";
        public string INSTANCEID = "MEHRSQLSERVER";
        public string TCPENABLED = "1";
        public string FilePath = "";
        public string PID = "HMWJ3-KY3J2-NMVD7-KG4JR-X2G8G"; //2019
        public bool ADDCURRENTUSERASSQLADMIN = true;
        public bool INDICATEPROGRESS = true;
        public bool USEMICROSOFTUPDATE = false;
        public delegate void InstallProgressEvent(int progress);
        public event InstallProgressEvent InstallProgressEventHnadler;
        #endregion

        private string BuildCommandLine()
        {
            StringBuilder strCommandLine = new StringBuilder();

            strCommandLine.Append("/qs "); //qs for show sql progress bar --- q does not show progress bar
            if (!IsNullOrEmpty(ACTION))
            {
                strCommandLine.Append("/ACTION=").Append(ACTION).Append(" ");
            }

            //strCommandLine.Append("/PID=").Append(PID).Append(" ");

            //
strCommandLine.Append("/IACCEPTSQLSERVERLICENSETERMS=").Append(IACCEPTSQLSERVERLICENSETERMS).Append(" ");

            strCommandLine.Append("/FEATURES=").Append(FEATURES).Append(" ");

            strCommandLine.Append("/SECURITYMODE=").Append(SECURITYMODE).Append(" ");

            strCommandLine.Append("/SAPWD=").Append(SAPWD).Append(" ");

            strCommandLine.Append("/INSTANCENAME=").Append(INSTANCENAME).Append(" ");

            strCommandLine.Append("/INSTANCEID=").Append(INSTANCEID).Append(" ");

            strCommandLine.Append("/ADDCURRENTUSERASSQLADMIN=").Append(ADDCURRENTUSERASSQLADMIN).Append(" ");

            strCommandLine.Append("/INDICATEPROGRESS=").Append(INDICATEPROGRESS).Append(" ");

            strCommandLine.Append("/USEMICROSOFTUPDATE=").Append(USEMICROSOFTUPDATE).Append(" ");
            return strCommandLine.ToString();
        }

        private string BuildCommandLineSql2014()
        {
            StringBuilder strCommandLine = new StringBuilder();

            strCommandLine.Append("/qs "); //qs for show sql progress bar --- q does not show progress bar
            if (!IsNullOrEmpty(ACTION))
            {
                strCommandLine.Append("/ACTION=").Append(ACTION).Append(" ");
            }

            strCommandLine.Append("/IACCEPTSQLSERVERLICENSETERMS ");//.Append(IACCEPTSQLSERVERLICENSETERMS.ToString().ToUpper()).Append(" ");

            strCommandLine.Append("/FEATURES=").Append(FEATURES).Append(" ");

            strCommandLine.Append("/SECURITYMODE=").Append(SECURITYMODE).Append(" ");

            strCommandLine.Append("/SAPWD=").Append(SAPWD).Append(" ");

            strCommandLine.Append("/INSTANCENAME=").Append(INSTANCENAME).Append(" ");

            strCommandLine.Append("/INSTANCEID=").Append(INSTANCEID).Append(" ");

            return strCommandLine.ToString();
        }
        public void InstallExpress()
        {
            try
            {
                string command = BuildCommandLineSql2014();
                Process InstallSql = Process.Start(FilePath, command);
                // InstallSql.WaitForExit();
                int progress = 0;
                do
                {
                    Thread.Sleep(1500);
                    if (progress != 99)
                    {
                        progress += 1;
                    }
                    InstallProgressEventHnadler(progress);

                } while (!InstallSql.HasExited );
                if (progress<99)
                {
                    for (int i = progress +1 ; i < 100; i++)
                    {
                        Thread.Sleep(1500);
                        InstallProgressEventHnadler(i);
                    }
                }
                InstallProgressEventHnadler(100);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool IsNullOrEmpty(string str)
        {
            return (str == null) || (str == string.Empty);
        }
    }
}
