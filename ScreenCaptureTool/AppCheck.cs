using System.Diagnostics;
using static ArnoldVinkCode.AVFunctions;

namespace ScreenCapture
{
    public partial class AppCheck
    {
        public static void Startup_Check()
        {
            try
            {
                Debug.WriteLine("Checking application status.");

                //Set the working directory to executable directory
                ApplicationUpdateWorkingPath();
            }
            catch { }
        }
    }
}