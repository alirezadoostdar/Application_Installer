using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    public class BugReportViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string App_Type { get; set; } = "Mehr Installer";
        public byte App_TypeId { get; set; } = 2;
        public string ServerShamsiDate { get; set; } = "";
        public DateTime ServerMiladiDate { get; set; } = DateTime.Today;
        public string UserName_Id { get; set; } = "";
        public string App_Version { get; set; } = "";
        public string DB_Version { get; set; } = "";
        public string Os_Version { get; set; } = "";
        public string Exe_Path { get; set; } = "";
        public bool Is_Client { get; set; } = false;
        public string ConnectionString { get; set; } = "";
        public string Server_Name { get; set; } = "";
        public string UserShamsiDate { get; set; } = "";
        public DateTime UserMiladiDate { get; set; }
        public string HID { get; set; } = "";
        public string Method_Name { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public string InnerException { get; set; } = "";
        public bool Checked { get; set; } = false;
        public string Version_Fixed { get; set; } = "";

    }
}
