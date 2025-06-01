using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mehr.Installer
{
    class Loading
    {
        public static IOverlaySplashScreenHandle Show(Control form)
        {
            IOverlaySplashScreenHandle handle = null;
            try
            {
                return SplashScreenManager.ShowOverlayForm(form, OverlayWindowOptions.Default);
            }
            catch (Exception)
            {
                return handle;
            }

        }
        public static void Close(IOverlaySplashScreenHandle handle)
        {
            try
            {
                SplashScreenManager.CloseOverlayForm(handle);
            }
            catch (Exception)
            {

            }
        }
    }
}
