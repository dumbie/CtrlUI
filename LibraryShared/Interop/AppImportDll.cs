using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace LibraryShared
{
    public partial class AppImport
    {
        //Application DllImports
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);
        [DllImport("kernel32.dll")]
        public static extern bool Wow64DisableWow64FsRedirection(IntPtr ptr);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder caption, int count);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr handle);
        [DllImport("kernel32.dll")]
        public static extern int GetApplicationUserModelId(IntPtr hProcess, ref int AppModelIDLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbAppUserModelID);
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        public static extern uint GetProcessIdOfThread(IntPtr hWnd);
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid iid, [Out(), MarshalAs(UnmanagedType.Interface)] out IPropertyStore propertyStore);

        //Window change monitor
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public const uint WINEVENT_OUTOFCONTEXT = 0;
        public const uint WINEVENT_SYSTEM_FOREGROUND = 3;

        //Open and close process
        [DllImport("Advapi32.dll")]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, DesiredAccessFlags desiredAccess, out IntPtr TokenHandle);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hHandle);
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
        public enum DesiredAccessFlags : uint
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_QUERY = 0x0008,
            TOKEN_ADJUST_DEFAULT = 0x0080
        }

        //Get process details
        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);
        [DllImport("Kernel32.dll")]
        public static extern bool QueryFullProcessImageName([In]IntPtr hProcess, [In]int dwFlags, [Out]StringBuilder lpExeName, ref uint lpdwSize);

        //Get Class Long
        public static IntPtr GetClassLongAuto(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr64(hWnd, nIndex);
            }
            else
            {
                return new IntPtr(GetClassLong32(hWnd, nIndex));
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        //Get and Set Window Long
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        public static IntPtr GetWindowLongAuto(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetWindowLongPtr64(hWnd, nIndex);
            }
            else
            {
                return new IntPtr(GetWindowLong32(hWnd, nIndex));
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        public static IntPtr SetWindowLongAuto(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }

        public enum WindowStyles : uint
        {
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xc00000,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0x0,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000u,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEFRAME = 0x40000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000
        }

        public enum WindowStylesEx : uint
        {
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_WINDOWEDGE = 0x00000100
        }
        public enum WindowLongFlags : int
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        //Get Window Position
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref WindowRectangle rectangle);
        public struct WindowRectangle
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        //Set Window Position
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        public enum WindowPosition
        {
            Top = 0,
            Bottom = 1,
            TopMost = -1,
            NoTopMost = -2
        }

        public enum WindowSWP
        {
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000
        }

        public enum WindowMessages
        {
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOUSEHWHEEL = 0x20E,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_SYSCOMMAND = 0x112,
            WM_COPYDATA = 0x004A,
            WM_HOTKEY = 0x312,
            WM_GETICON = 0x7F,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14,
            WM_CLOSE = 0x0010,
            WM_QUIT = 0x0012
        }

        //Send message or key
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        //Keyboard events
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        public const uint KEYEVENTF_NONE = 0x0;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        public const uint MAPVK_VK_TO_VSC = 0x00;
        public const uint MAPVK_VSC_TO_VK = 0x01;
        public const uint MAPVK_VK_TO_CHAR = 0x02;
        public const uint MAPVK_VSC_TO_VK_EX = 0x03;
        public const uint MAPVK_VK_TO_VSC_EX = 0x04;

        //Mouse events
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointWin lpPoint);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        public enum MouseEvents
        {
            MOUSEEVENTF_MOVE = 0x01,
            MOUSEEVENTF_LEFTDOWN = 0x02,
            MOUSEEVENTF_LEFTUP = 0x04,
            MOUSEEVENTF_RIGHTDOWN = 0x08,
            MOUSEEVENTF_RIGHTUP = 0x10,
            MOUSEEVENTF_MIDDLEDOWN = 0x20,
            MOUSEEVENTF_MIDDLEUP = 0x40,
            MOUSEEVENTF_VWHEEL = 0x0800,
            MOUSEEVENTF_HWHEEL = 0x01000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointWin
        {
            public int X;
            public int Y;
            public static implicit operator Point(PointWin point)
            {
                return new Point(point.X, point.Y);
            }
        }

        //Register Hotkey
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public static int HotKeyRegisterId = 0x9000;

        //Window Placement
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);
        public struct WindowPlacement
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }
        public enum WindowFlags
        {
            NoRequest = 0,
            SetMinPosition = 1,
            RestoreToMaximized = 2,
            AsyncWindowPlacement = 3
        }
        public enum WindowShowCommand
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }
    }
}