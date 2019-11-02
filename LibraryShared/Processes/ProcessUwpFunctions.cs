using ArnoldVinkCode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using static LibraryShared.AppImport;
using static LibraryShared.Classes;

namespace LibraryShared
{
    public partial class Processes
    {
        //Launch an uwp application manually
        public static void ProcessLauncherUwp(string PathExe, string Argument)
        {
            try
            {
                //Show launching message
                Debug.WriteLine("Launching UWP: " + Path.GetFileNameWithoutExtension(PathExe));

                //Prepare the launching task
                void TaskAction()
                {
                    try
                    {
                        UWPActivationManager UWPActivationManager = new UWPActivationManager();
                        UWPActivationManager.ActivateApplication(PathExe, Argument, UWPActivationManagerOptions.None, out int ProcessId);
                    }
                    catch { }
                }

                //Launch the application
                AVActions.TaskStart(TaskAction, null);
            }
            catch
            {
                Debug.WriteLine("Failed launching UWP: " + Path.GetFileNameWithoutExtension(PathExe));
            }
        }

        //Check if a window is an uwp application
        public static bool CheckProcessIsUwp(IntPtr TargetWindowHandle)
        {
            try
            {
                string ClassNamestring = GetClassNameFromWindowHandle(TargetWindowHandle);
                if (ClassNamestring == "ApplicationFrameWindow" || ClassNamestring == "Windows.UI.Core.CoreWindow")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }

        //Get an uwp application window from CoreWindowHandle
        public static IntPtr GetUwpWindowFromCoreWindowHandle(IntPtr TargetCoreWindowHandle)
        {
            try
            {
                Process AllProcess = GetProcessByName("ApplicationFrameHost", false);
                if (AllProcess != null)
                {
                    foreach (ProcessThread ThreadProcess in AllProcess.Threads)
                    {
                        foreach (IntPtr ThreadWindowHandle in EnumThreadWindows(ThreadProcess.Id))
                        {
                            try
                            {
                                //Get class name
                                string ClassNameString = GetClassNameFromWindowHandle(ThreadWindowHandle);

                                //Get information from frame window
                                if (ClassNameString == "ApplicationFrameWindow")
                                {
                                    IntPtr ThreadWindowHandleEx = FindWindowEx(ThreadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                    if (ThreadWindowHandleEx == TargetCoreWindowHandle)
                                    {
                                        return ThreadWindowHandle;
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            return IntPtr.Zero;
        }

        //Get an uwp application process from AppUserModelId
        public static List<ProcessUwp> GetUwpProcessFromAppUserModelId(string TargetAppUserModelId)
        {
            List<ProcessUwp> UwpAppList = new List<ProcessUwp>();
            try
            {
                Process AllProcess = GetProcessByName("ApplicationFrameHost", false);
                if (AllProcess != null)
                {
                    foreach (ProcessThread ThreadProcess in AllProcess.Threads)
                    {
                        try
                        {
                            int ProcessId = -1;
                            IntPtr WindowHandle = IntPtr.Zero;
                            bool ProcessUserRun = false;

                            foreach (IntPtr ThreadWindowHandle in EnumThreadWindows(ThreadProcess.Id))
                            {
                                try
                                {
                                    //Get class name
                                    string ClassNameString = GetClassNameFromWindowHandle(ThreadWindowHandle);

                                    //Get information from frame window
                                    if (ClassNameString == "ApplicationFrameWindow")
                                    {
                                        //Get window handle
                                        WindowHandle = ThreadWindowHandle;

                                        //Get process id
                                        IntPtr ThreadWindowHandleEx = FindWindowEx(ThreadWindowHandle, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
                                        if (ThreadWindowHandleEx != IntPtr.Zero)
                                        {
                                            GetWindowThreadProcessId(ThreadWindowHandleEx, out ProcessId);
                                        }
                                    }

                                    //Check if user started uwp application
                                    if (ClassNameString == "MSCTFIME UI")
                                    {
                                        ProcessUserRun = true;
                                    }
                                }
                                catch { }
                            }

                            if (ProcessUserRun)
                            {
                                string ProcessExecutablePath = GetAppUserModelIdFromWindowHandle(WindowHandle);
                                if (TargetAppUserModelId == ProcessExecutablePath)
                                {
                                    ProcessUwp uwpAppWindow = new ProcessUwp
                                    {
                                        ProcessId = ProcessId,
                                        WindowHandle = WindowHandle,
                                        AppUserModelId = ProcessExecutablePath
                                    };
                                    UwpAppList.Add(uwpAppWindow);
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return UwpAppList;
        }

        //Get details from an uwp package
        public static void GetUwpAppDetailsFromPackage(Package AppPackage, ref string AppName, ref string AppImagePath, ref string AppIdentifier)
        {
            try
            {
                //Get registery group information
                using (RegistryKey RegisteryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Get provided package details
                    string AppFamilyName = AppPackage.Id.FamilyName;
                    string AppFullname = AppPackage.Id.FullName;

                    //Get the application details
                    string RegisteryLocationDetails = @"Software\Classes\Extensions\ContractId\Windows.Launch\PackageId\" + AppFullname + @"\ActivatableClassId\";
                    using (RegistryKey RegisteryKeyDetails = RegisteryKeyCurrentUser.OpenSubKey(RegisteryLocationDetails))
                    {
                        if (RegisteryKeyDetails != null)
                        {
                            foreach (string RegisterySub in RegisteryKeyDetails.GetSubKeyNames())
                            {
                                try
                                {
                                    //Set the application identifier
                                    AppIdentifier = AppFamilyName + "!" + RegisterySub;

                                    //Get the application display name and image
                                    using (RegistryKey RegisterySubKeyDetails = RegisteryKeyCurrentUser.OpenSubKey(RegisteryLocationDetails + RegisterySub))
                                    {
                                        if (RegisterySubKeyDetails != null)
                                        {
                                            try
                                            {
                                                AppName = RegisterySubKeyDetails.GetValue("DisplayName").ToString();
                                                if (AppName.StartsWith("@{")) { AppName = ConvertIndirectString(AppName); }
                                            }
                                            catch { }
                                            try
                                            {
                                                AppImagePath = RegisterySubKeyDetails.GetValue("Icon").ToString();
                                                if (AppImagePath.StartsWith("@{")) { AppImagePath = ConvertIndirectString(AppImagePath); }
                                            }
                                            catch { }
                                            return;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }

                    //Get the application splash
                    string RegisteryLocationSplash = @"Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\SystemAppData\" + AppFamilyName + @"\SplashScreen\";
                    using (RegistryKey RegisteryKeySplash = RegisteryKeyCurrentUser.OpenSubKey(RegisteryLocationSplash))
                    {
                        if (RegisteryKeySplash != null)
                        {
                            foreach (string RegisterySub in RegisteryKeySplash.GetSubKeyNames())
                            {
                                try
                                {
                                    //Set the application identifier
                                    AppIdentifier = RegisterySub;

                                    //Get the application display name and image
                                    using (RegistryKey RegisterySubKeySplash = RegisteryKeyCurrentUser.OpenSubKey(RegisteryLocationSplash + RegisterySub))
                                    {
                                        if (RegisterySubKeySplash != null)
                                        {
                                            try
                                            {
                                                AppName = RegisterySubKeySplash.GetValue("AppName").ToString();
                                                if (AppName.StartsWith("@{")) { AppName = ConvertIndirectString(AppName); }
                                            }
                                            catch { }
                                            try
                                            {
                                                AppImagePath = RegisterySubKeySplash.GetValue("Image").ToString();
                                                if (AppImagePath.StartsWith("@{")) { AppImagePath = ConvertIndirectString(AppImagePath); }
                                            }
                                            catch { }
                                            return;
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        //Get uwp application image
        public static string GetUwpAppImagePath(string AppUserModelId)
        {
            try
            {
                //Get search information
                string[] UserModelSplit = AppUserModelId.Split('!');
                string AppFamily = UserModelSplit[0];
                string AppIdentifier = UserModelSplit[1];
                string AppName = string.Empty;
                string AppImagePath = string.Empty;

                //Get all the installed UWP apps
                PackageManager DeployPackageManager = new PackageManager();
                string CurrentUserIdentity = WindowsIdentity.GetCurrent().User.Value;
                IEnumerable<Package> AppPackages = DeployPackageManager.FindPackagesForUser(CurrentUserIdentity);
                Package AppPackage = AppPackages.Where(x => x.Id.FamilyName == AppFamily).FirstOrDefault();
                if (AppPackage != null)
                {
                    //Get detailed application information
                    GetUwpAppDetailsFromPackage(AppPackage, ref AppName, ref AppImagePath, ref AppIdentifier);
                    return AppImagePath;
                }
            }
            catch { }
            return string.Empty;
        }

        //Convert Indirect UWP application information to string
        public static string ConvertIndirectString(string IndirectString)
        {
            try
            {
                StringBuilder IndirectStringBuild = new StringBuilder(1024);
                SHLoadIndirectString(IndirectString, IndirectStringBuild, IndirectStringBuild.Capacity, IntPtr.Zero);
                return IndirectStringBuild.ToString();
            }
            catch { }
            return string.Empty;
        }
    }
}