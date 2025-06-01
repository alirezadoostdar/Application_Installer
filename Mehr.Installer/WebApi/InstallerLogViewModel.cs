using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    public class InstallerLogViewModel
    {
        public int Id { get; set; } = 0;
        public string ShamsiDate { get; set; } = DateTime.Now.ToShamsi();
        public DateTime MildaiDate { get; set; } = DateTime.Now;
        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
        public bool IsSuccess { get; set; } = true;
        public string Error { get; set; } = "aa";
        public int ErrorId { get; set; } = 0;
        public string Client_Ip { get; set; } = "a";
        public string City { get; set; } = "a";
        public string Country { get; set; } = "a";
        public string CountryCode { get; set; } = "a";
        public string Isp { get; set; } = "a";
        public string Lat { get; set; } = "a";
        public string Lon { get; set; } = "a";
        public string Org { get; set; } = "a";
        public string Region { get; set; } = "a";
        public string RegionName { get; set; } = "a";
        public string Timezone { get; set; } = "a";
    }
}
