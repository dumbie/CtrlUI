using ArnoldVinkCode;
using System.Configuration;
using static ArnoldVinkCode.AVSettings;

namespace ScreenCapture
{
    public partial class AppVariables
    {
        //Application Variables
        public static Configuration vConfiguration = SettingLoadConfig("ScreenCaptureTool.exe.csettings");

        //Setting Variables
        public static bool vComboboxSaveEnabled = true;

        //Sockets Variables
        public static ArnoldVinkSockets vArnoldVinkSockets = null;

        //Pipes Variables
        public static ArnoldVinkPipes vArnoldVinkPipes = null;

        //Capture Variables
        public static string vCaptureFileName = string.Empty;

        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();
        public static WindowOverlay vWindowOverlay = new WindowOverlay();
    }
}