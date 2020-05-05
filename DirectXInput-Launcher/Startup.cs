using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;
using static LibraryShared.AppStartupCheck;

namespace AdminLauncher
{
    public partial class App : Application
    {
        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check the application status
                await Application_LaunchCheck("DirectXInput Launcher", ProcessPriorityClass.Normal, false, false);

                //Enable launch requirements
                InstallCertificate(@"Resources\ArnoldVinkCertificate.cer");
                bool secureUIAEnabled = SecureUIAPathsCheck();
                if (!secureUIAEnabled)
                {
                    SecureUIAPathsAllow();
                }

                //Run the certified application
                Process launchProcess = await ProcessLauncherWin32Async("DirectXInput.exe", "", "", false, false);
                if (launchProcess == null)
                {
                    MessageBox.Show("Failed launching the application.", "DirectXInput Launcher");
                }

                //Disable launch requirements
                if (!secureUIAEnabled)
                {
                    await Task.Delay(5000);
                    SecureUIAPathsBlock();
                }

                Debug.WriteLine("Launcher finished.");
                Environment.Exit(0);
                return;
            }
            catch { }
        }

        bool SecureUIAPathsCheck()
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyPolicies = registryKeyLocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System\", true))
                    {
                        return regKeyPolicies.GetValue("EnableSecureUIAPaths").ToString() == "0" ? true : false;
                    }
                }
            }
            catch { }
            return false;
        }

        void SecureUIAPathsAllow()
        {
            try
            {
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyPolicies = registryKeyLocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System\", true))
                    {
                        regKeyPolicies.SetValue("EnableSecureUIAPaths", 0);
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
                using (RegistryKey registryKeyLocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (RegistryKey regKeyPolicies = registryKeyLocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System\", true))
                    {
                        regKeyPolicies.SetValue("EnableSecureUIAPaths", 1);
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