using Microsoft.Win32;

namespace CtrlUI
{
    public partial class WindowMain
    {
        //Registry enable linked connections
        public void RegistryEnableLinkedConnections()
        {
            try
            {
                using (RegistryKey RegisteryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey OpenSubKey = RegisteryKeyLocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true))
                    {
                        OpenSubKey.SetValue("EnableLinkedConnections", 1);
                    }
                }
            }
            catch { }
        }
    }
}