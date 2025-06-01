using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    internal class DataTools
    {
        public static bool IsNullOrEmpty(string str)
        {
            return (str == null) || (str == string.Empty);
        }
    }
}
