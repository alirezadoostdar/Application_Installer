using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    public class Info
    {
        public static int GetOperationSystemBit()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return 64;
            }
            else
            {
                return 32;
            }
        }
    }
}
