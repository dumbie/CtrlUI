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
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyLocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true))
                    {
                        openSubKey.SetValue("EnableLinkedConnections", 1);
                    }
                }
            }
            catch { }
        }
    }
}