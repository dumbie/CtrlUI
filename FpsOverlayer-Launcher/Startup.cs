using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;
using static LibraryShared.AppLaunchCheck;

namespace AdminLauncher
{
    public partial class App : Application
    {
        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check application status
                Application_LaunchCheck("Fps Overlayer Launcher", "FpsOverlayer-Launcher", false, false);

                //Enable launch requirements
                InstallCertificate(@"Resources\ArnoldVinkCertificate.cer");
                SecureUIAPathsAllow();

                //Run the fps overlayer
                ProcessLauncherWin32("FpsOverlayer.exe", "", "", false, false);

                //Disable launch requirements
                await Task.Delay(5000);
                SecureUIAPathsBlock();

                Debug.WriteLine("Launcher finished.");
                Environment.Exit(0);
                return;
            }
            catch { }
        }

        void SecureUIAPathsAllow()
        {
            try
            {
                using (RegistryKey RegisteryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey RegKeyPolicies = RegisteryKeyLocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System\", true))
                    {
                        RegKeyPolicies.SetValue("EnableSecureUIAPaths", 0);
                    }
                }

                Debug.WriteLine("Disabled the secure uia paths check.");
            }
            catch { }
        }

        void SecureUIAPathsBlock()
        {
            try
            {
                using (RegistryKey RegisteryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey RegKeyPolicies = RegisteryKeyLocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System\", true))
                    {
                        RegKeyPolicies.SetValue("EnableSecureUIAPaths", 1);
                    }
                }

                Debug.WriteLine("Enabled the secure uia paths check.");
            }
            catch { }
        }

        void InstallCertificate(string CertificateFilename)
        {
            try
            {
                X509Certificate2 certificateFile = new X509Certificate2(CertificateFilename);
                X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

                certificateStore.Open(OpenFlags.ReadWrite);
                certificateStore.Add(certificateFile);
                certificateStore.Close();

                Debug.WriteLine("Registered certificate to root.");
            }
            catch { }
        }
    }
}