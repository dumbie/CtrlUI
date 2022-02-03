using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropDll;

namespace FpsOverlayer
{
    public class TaskbarInformation
    {
        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(AppBarMessages dwMessage, ref AppBarData pData);

        private enum AppBarMessages : int
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        private enum AppBarStates : int
        {
            ABS_MANUAL = 0,
            ABS_AUTOHIDE = 1,
            ABS_ALWAYSONTOP = 2,
            ABS_AUTOHIDEANDONTOP = 3
        }

        public enum AppBarPosition : int
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        private struct AppBarData
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public AppBarPosition uEdge;
            public WindowRectangle rc;
            public int lParam;
        }

        public WindowRectangle Bounds { get; set; }
        public AppBarPosition Position { get; set; }
        public bool IsAutoHide { get; set; }
        public bool IsVisible { get; set; }

        public TaskbarInformation()
        {
            try
            {
                IntPtr taskBarHandle = FindWindow("Shell_TrayWnd", null);

                //Get window rectangle
                WindowRectangle windowRectangle = new WindowRectangle();
                GetWindowRect(taskBarHandle, ref windowRectangle);

                //Create AppBarData
                AppBarData taskBarData = new AppBarData();
                taskBarData.cbSize = (uint)Marshal.SizeOf(typeof(AppBarData));
                taskBarData.hWnd = taskBarHandle;

                //Get taskbar position
                IntPtr taskBarMessage = SHAppBarMessage(AppBarMessages.ABM_GETTASKBARPOS, ref taskBarData);
                if (taskBarMessage == IntPtr.Zero)
                {
                    Debug.WriteLine("Failed to get taskbar position.");
                    return;
                }

                //Get taskbar state
                int taskBarState = SHAppBarMessage(AppBarMessages.ABM_GETSTATE, ref taskBarData).ToInt32();

                //Set taskbar result
                Position = taskBarData.uEdge;
                Bounds = taskBarData.rc;
                IsAutoHide = (taskBarState & (int)AppBarStates.ABS_AUTOHIDE) == (int)AppBarStates.ABS_AUTOHIDE;
                if (Position == AppBarPosition.ABE_TOP || Position == AppBarPosition.ABE_BOTTOM)
                {
                    IsVisible = Bounds.Top == windowRectangle.Top;
                }
                else
                {
                    IsVisible = Bounds.Left == windowRectangle.Left;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get taskbar information: " + ex.Message);
            }
        }
    }
}