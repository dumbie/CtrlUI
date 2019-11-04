using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using static LibraryShared.AppImport;
using static LibraryShared.Classes;
using static LibraryShared.ProcessNtQueryInformation;

namespace LibraryShared
{
    public partial class Processes
    {
        //Focus on an application window handle
        public static async Task<bool> FocusWindowHandle(string TitleTarget, IntPtr TargetWindowHandle, int ShowCommand, bool SetWindowState, bool SwitchWindow, bool ToTopWindow, bool SetForeground, bool TempTopMost)
        {
            try
            {
                //Detect the previous window state
                if (ShowCommand == 0 && SetWindowState)
                {
                    WindowPlacement ProcessWindowState = new WindowPlacement();
                    GetWindowPlacement(TargetWindowHandle, ref ProcessWindowState);
                    Debug.WriteLine("Detected the previous window state: " + ProcessWindowState.Flags);
                    if (ProcessWindowState.Flags == (int)WindowFlags.RestoreToMaximized)
                    {
                        ShowCommand = (int)WindowShowCommand.ShowMaximized;
                    }
                    else
                    {
                        ShowCommand = (int)WindowShowCommand.Restore;
                    }
                }

                //Enable the process window as top most
                if (TempTopMost)
                {
                    SetWindowPos(TargetWindowHandle, (IntPtr)WindowPosition.TopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                    await Task.Delay(10);
                }

                //Start focusing on the application
                if (SetWindowState)
                {
                    ShowWindow(TargetWindowHandle, ShowCommand);
                    await Task.Delay(10);
                }

                if (SwitchWindow)
                {
                    SwitchToThisWindow(TargetWindowHandle, true);
                    await Task.Delay(10);
                }

                if (ToTopWindow)
                {
                    BringWindowToTop(TargetWindowHandle);
                    await Task.Delay(10);
                }

                if (SetForeground)
                {
                    SetForegroundWindow(TargetWindowHandle);
                    await Task.Delay(10);
                }

                //Disable the process window as top most
                if (TempTopMost)
                {
                    Debug.WriteLine("Disabling top most from process: " + TitleTarget);
                    SetWindowPos(TargetWindowHandle, (IntPtr)WindowPosition.NoTopMost, 0, 0, 0, 0, (int)WindowSWP.NOMOVE | (int)WindowSWP.NOSIZE);
                }

                Debug.WriteLine("Changed process window: " + TitleTarget + " WindowHandle: " + TargetWindowHandle + " ShowCmd: " + ShowCommand);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed showing the application, perhaps it is no longer running?");
                return false;
            }
        }

        //Enumerate all thread windows including fullscreen
        public static List<IntPtr> EnumThreadWindows(int threadId)
        {
            IntPtr childWindow = IntPtr.Zero;
            List<IntPtr> listIntPtr = new List<IntPtr>();
            try
            {
                while ((childWindow = FindWindowEx(IntPtr.Zero, childWindow, null, null)) != IntPtr.Zero)
                {
                    try
                    {
                        if (GetWindowThreadProcessId(childWindow, out int processId) == threadId)
                        {
                            listIntPtr.Add(childWindow);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return listIntPtr;
        }

        //Check if a specific process is running by id
        public static bool CheckRunningProcessById(int ProcessId)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.Id == ProcessId);
            }
            catch { return false; }
        }

        //Check if a specific process is running by window handle
        public static bool CheckRunningProcessByWindowHandle(IntPtr WindowHandle)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.MainWindowHandle == WindowHandle);
            }
            catch { return false; }
        }

        //Check if a specific process is running by name
        public static bool CheckRunningProcessByName(string ProcessName, bool WindowTitle)
        {
            try
            {
                if (WindowTitle) { return Process.GetProcesses().Any(x => x.MainWindowTitle.ToLower().Contains(ProcessName.ToLower())); }
                else { return Process.GetProcessesByName(ProcessName).Any(); }
            }
            catch { return false; }
        }

        //Get the window title from process
        public static string GetWindowTitleFromProcess(Process TargetProcess)
        {
            string ProcessTitle = "Unknown";
            try
            {
                ProcessTitle = TargetProcess.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(ProcessTitle)) { ProcessTitle = GetWindowTitleFromWindowHandle(TargetProcess.MainWindowHandle); }
                if (string.IsNullOrWhiteSpace(ProcessTitle)) { ProcessTitle = TargetProcess.ProcessName; }
                if (!string.IsNullOrWhiteSpace(ProcessTitle))
                {
                    ProcessTitle = AVFunctions.StringRemoveStart(ProcessTitle, " ");
                    ProcessTitle = AVFunctions.StringRemoveEnd(ProcessTitle, " ");
                }
                else
                {
                    ProcessTitle = "Unknown";
                }
            }
            catch { }
            return ProcessTitle;
        }

        //Get the class name from window handle
        public static string GetClassNameFromWindowHandle(IntPtr TargetWindowHandle)
        {
            try
            {
                StringBuilder ClassNameBuilder = new StringBuilder(256);
                GetClassName(TargetWindowHandle, ClassNameBuilder, ClassNameBuilder.Capacity);
                return ClassNameBuilder.ToString();
            }
            catch { return string.Empty; }
        }

        //Get the window title from window handle
        public static string GetWindowTitleFromWindowHandle(IntPtr TargetWindowHandle)
        {
            string ProcessTitle = "Unknown";
            try
            {
                int WindowTextBuilderLength = GetWindowTextLength(TargetWindowHandle) + 1;
                StringBuilder WindowTextBuilder = new StringBuilder(WindowTextBuilderLength);
                GetWindowText(TargetWindowHandle, WindowTextBuilder, WindowTextBuilder.Capacity);
                string BuilderString = WindowTextBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(BuilderString))
                {
                    ProcessTitle = BuilderString;
                    ProcessTitle = AVFunctions.StringRemoveStart(ProcessTitle, " ");
                    ProcessTitle = AVFunctions.StringRemoveEnd(ProcessTitle, " ");
                }
                else
                {
                    ProcessTitle = "Unknown";
                }
            }
            catch { }
            return ProcessTitle;
        }

        //Get the currently focused process
        public static ProcessFocus GetFocusedProcess()
        {
            try
            {
                AutomationElement FocusedSource = AutomationElement.FocusedElement;
                ProcessFocus processFocus = new ProcessFocus();
                processFocus.Process = GetProcessById(FocusedSource.Current.ProcessId);

                //Get window handle
                if (processFocus.Process.MainWindowHandle != IntPtr.Zero)
                {
                    processFocus.WindowHandle = processFocus.Process.MainWindowHandle;
                }
                else if (FocusedSource.Current.NativeWindowHandle != 0)
                {
                    if (FocusedSource.Current.ClassName == "ApplicationFrameWindow" || FocusedSource.Current.ClassName == "Windows.UI.Core.CoreWindow")
                    {
                        processFocus.WindowHandle = GetUwpWindowFromCoreWindowHandle(new IntPtr(FocusedSource.Current.NativeWindowHandle));
                    }
                    else
                    {
                        processFocus.WindowHandle = new IntPtr(FocusedSource.Current.NativeWindowHandle);
                    }
                }
                else if (GetForegroundWindow() != IntPtr.Zero)
                {
                    processFocus.WindowHandle = GetForegroundWindow();
                }

                //Get window title
                if (!string.IsNullOrWhiteSpace(processFocus.Process.MainWindowTitle))
                {
                    processFocus.Title = GetWindowTitleFromProcess(processFocus.Process);
                }
                else
                {
                    processFocus.Title = GetWindowTitleFromWindowHandle(processFocus.WindowHandle);
                }

                return processFocus;
            }
            catch { return null; }
        }

        //Get a process by id safe return null
        public static Process GetProcessById(int ProcessId)
        {
            try
            {
                return Process.GetProcessById(ProcessId);
            }
            catch { return null; }
        }

        //Get a single specific process by name or title
        public static Process GetProcessByName(string ProcessName, bool WindowTitle)
        {
            try
            {
                if (WindowTitle)
                {
                    foreach (Process AllProcess in Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(ProcessName.ToLower())))
                    {
                        return AllProcess;
                    }
                }
                else
                {
                    foreach (Process AllProcess in Process.GetProcessesByName(ProcessName))
                    {
                        return AllProcess;
                    }
                }
                return null;
            }
            catch { return null; }
        }

        //Get multiple specific processes by name or title
        public static Process[] GetProcessesByName(string ProcessName, bool WindowTitle, bool ExactName)
        {
            try
            {
                if (WindowTitle)
                {
                    return Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(ProcessName.ToLower())).ToArray();
                }
                else
                {
                    if (ExactName) { return Process.GetProcessesByName(ProcessName); }
                    else { return Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(ProcessName.ToLower())).ToArray(); }
                }
            }
            catch { return null; }
        }

        //Close processes by name or window title
        public static bool CloseProcessesByName(string ProcessName, bool WindowTitle)
        {
            try
            {
                if (WindowTitle)
                {
                    foreach (Process AllProcess in Process.GetProcesses().Where(x => x.MainWindowTitle.ToLower().Contains(ProcessName.ToLower())))
                    {
                        AllProcess.Kill();
                    }
                }
                else
                {
                    foreach (Process AllProcess in Process.GetProcessesByName(ProcessName))
                    {
                        AllProcess.Kill();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        //Close process by id
        public static bool CloseProcessById(int ProcessId)
        {
            try
            {
                Debug.WriteLine("Closing process by id: " + ProcessId);
                GetProcessById(ProcessId).Kill();
                return true;
            }
            catch { return false; }
        }

        //Close process by window handle
        public static bool CloseProcessByWindowHandle(IntPtr WindowHandle)
        {
            try
            {
                Debug.WriteLine("Closing process by window handle: " + WindowHandle);
                SendMessage(WindowHandle, (int)WindowMessages.WM_CLOSE, 0, 0);
                SendMessage(WindowHandle, (int)WindowMessages.WM_QUIT, 0, 0);
                return true;
            }
            catch { return false; }
        }

        //Get the full exe path from process
        public static string GetExecutablePathFromProcess(Process TargetProcess)
        {
            string ExePath = string.Empty;
            try
            {
                ExePath = TargetProcess.MainModule.FileName;
            }
            catch { }
            if (string.IsNullOrWhiteSpace(ExePath))
            {
                try
                {
                    uint stringLength = 1024;
                    StringBuilder stringBuilder = new StringBuilder((int)stringLength);
                    bool Succes = QueryFullProcessImageName(TargetProcess.Handle, 0, stringBuilder, ref stringLength);
                    if (Succes) { ExePath = stringBuilder.ToString(); }
                }
                catch { }
            }
            return ExePath;
        }

        //Get the AppUserModelId from process
        public static string GetAppUserModelIdFromProcess(Process TargetProcess)
        {
            string AppUserModelId = string.Empty;
            try
            {
                int stringLength = 256;
                StringBuilder stringBuilder = new StringBuilder(stringLength);
                int Succes = GetApplicationUserModelId(TargetProcess.Handle, ref stringLength, stringBuilder);
                if (Succes == 0) { AppUserModelId = stringBuilder.ToString(); }
            }
            catch { }
            return AppUserModelId;
        }

        //Get the AppUserModelId from window handle
        public static string GetAppUserModelIdFromWindowHandle(IntPtr TargetWindowHandle)
        {
            try
            {
                PropertyVariant propertyVariant = new PropertyVariant();
                Guid propertyStoreGuid = typeof(IPropertyStore).GUID;

                SHGetPropertyStoreForWindow(TargetWindowHandle, ref propertyStoreGuid, out IPropertyStore propertyStore);
                propertyStore.GetValue(ref PKEY_AppUserModel_ID, out propertyVariant);

                return Marshal.PtrToStringUni(propertyVariant.pwszVal);
            }
            catch { return string.Empty; }
        }

        //Get the launch arguments from process
        public static string GetLaunchArgumentsFromProcess(Process TargetProcess, string ExecutablePath)
        {
            string LaunchArguments = string.Empty;
            try
            {
                string RemoveFromArgument = '"' + ExecutablePath + '"';
                USER_PROCESS_PARAMETERS processParameter = USER_PROCESS_PARAMETERS.CommandLine;
                LaunchArguments = GetProcessParameterstring(TargetProcess.Id, processParameter);
                LaunchArguments = AVFunctions.StringReplaceFirst(LaunchArguments, RemoveFromArgument, string.Empty, true);
                LaunchArguments = AVFunctions.StringRemoveStart(LaunchArguments, " ");
            }
            catch { }
            return LaunchArguments;
        }

        //Check if process is active
        public static bool ValidateProcessState(Process TargetProcess, bool CheckSuspended, bool CheckWin32)
        {
            try
            {
                //Check if the application is suspended
                if (CheckSuspended)
                {
                    try
                    {
                        if (TargetProcess.Threads[0].WaitReason == ThreadWaitReason.Suspended)
                        {
                            //Debug.WriteLine("Application is suspended and can't be shown or hidden.");
                            return false;
                        }
                    }
                    catch { }
                }

                //Check if the application is win32
                if (CheckWin32)
                {
                    if (CheckProcessIsUwp(TargetProcess.MainWindowHandle))
                    {
                        //Debug.WriteLine("Application is an uwp application.");
                        return false;
                    }
                }

                return true;
            }
            catch { }
            return false;
        }

        //Check if window handle is a window
        public static bool ValidateWindowHandle(IntPtr TargetWindowHandle)
        {
            try
            {
                //Check if is a window
                if (!IsWindow(TargetWindowHandle))
                {
                    //Debug.WriteLine("Window handle is not a Window.");
                    return false;
                }

                //Check if window is visible
                if (!IsWindowVisible(TargetWindowHandle))
                {
                    //Debug.WriteLine("Window handle is not visible.");
                    return false;
                }

                //Check if application is hidden to the tray
                WindowPlacement ProcessWindowState = new WindowPlacement();
                GetWindowPlacement(TargetWindowHandle, ref ProcessWindowState);
                if (ProcessWindowState.ShowCmd <= 0)
                {
                    //Debug.WriteLine("Application is in the tray and can't be shown or hidden.");
                    return false;
                }

                ////Check if the window size is not zero
                //WindowPosition_Rectangle PositionRect = new WindowPosition_Rectangle();
                //GetWindowRect(TargetWindowHandle, ref PositionRect);
                //int WindowWidth = PositionRect.Right - PositionRect.Left;
                //int WindowHeight = PositionRect.Bottom - PositionRect.Top;
                //if (WindowWidth < 25 && WindowHeight < 25)
                //{
                //    Debug.WriteLine("Window is too small to be a proper window.");
                //    return false;
                //}

                ////Check the process window style
                //WindowStyles CurrentStyle = (WindowStyles)GetWindowLongAuto(TargetWindowHandle, (int)WindowLongFlags.GWL_STYLE).ToInt64();
                //if (!CurrentStyle.HasFlag(WindowStyles.WS_VISIBLE))
                //{
                //    Debug.WriteLine("Handle is missing WS_VISIBLE and can't be shown.");
                //    return false;
                //}

                return true;
            }
            catch { }
            return false;
        }
    }
}