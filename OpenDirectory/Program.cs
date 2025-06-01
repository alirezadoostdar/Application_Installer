using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            Process.Start("explorer.exe", path);
            Process.Start("https://mehraccounting.com/document/Index/1");
        }
    }
}
