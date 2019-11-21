using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.ProcessClasses;

namespace LibraryShared
{
    public partial class Classes
    {
        public class DataBindApp : INotifyPropertyChanged
        {
            private int PrivNumber = -1;
            public int Number
            {
                get { return this.PrivNumber; }
                set
                {
                    if (this.PrivNumber != value)
                    {
                        this.PrivNumber = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private AppCategory PrivCategory;
            public AppCategory Category
            {
                get { return this.PrivCategory; }
                set
                {
                    if (this.PrivCategory != value)
                    {
                        this.PrivCategory = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private ProcessType PrivType;
            public ProcessType Type
            {
                get { return this.PrivType; }
                set
                {
                    if (this.PrivType != value)
                    {
                        this.PrivType = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivName;
            public string Name
            {
                get { return this.PrivName; }
                set
                {
                    if (this.PrivName != value)
                    {
                        this.PrivName = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivNameExe;
            public string NameExe
            {
                get { return this.PrivNameExe; }
                set
                {
                    if (this.PrivNameExe != value)
                    {
                        this.PrivNameExe = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivPathImage;
            public string PathImage
            {
                get { return this.PrivPathImage; }
                set
                {
                    if (this.PrivPathImage != value)
                    {
                        this.PrivPathImage = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivPathExe;
            public string PathExe
            {
                get { return this.PrivPathExe; }
                set
                {
                    if (this.PrivPathExe != value)
                    {
                        this.PrivPathExe = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivPathLaunch;
            public string PathLaunch
            {
                get { return this.PrivPathLaunch; }
                set
                {
                    if (this.PrivPathLaunch != value)
                    {
                        this.PrivPathLaunch = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivPathRoms;
            public string PathRoms
            {
                get { return this.PrivPathRoms; }
                set
                {
                    if (this.PrivPathRoms != value)
                    {
                        this.PrivPathRoms = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivArgument;
            public string Argument
            {
                get { return this.PrivArgument; }
                set
                {
                    if (this.PrivArgument != value)
                    {
                        this.PrivArgument = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivLaunchFilePicker;
            public bool LaunchFilePicker
            {
                get { return this.PrivLaunchFilePicker; }
                set
                {
                    if (this.PrivLaunchFilePicker != value)
                    {
                        this.PrivLaunchFilePicker = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivLaunchKeyboard;
            public bool LaunchKeyboard
            {
                get { return this.PrivLaunchKeyboard; }
                set
                {
                    if (this.PrivLaunchKeyboard != value)
                    {
                        this.PrivLaunchKeyboard = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivQuickLaunch;
            public bool QuickLaunch
            {
                get { return this.PrivQuickLaunch; }
                set
                {
                    if (this.PrivQuickLaunch != value)
                    {
                        this.PrivQuickLaunch = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivRunningTime = -1;
            public int RunningTime
            {
                get { return this.PrivRunningTime; }
                set
                {
                    if (this.PrivRunningTime != value)
                    {
                        this.PrivRunningTime = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Status variables (no saving needed)
            private int PrivRunningTimeLastUpdate = 0;
            public int RunningTimeLastUpdate
            {
                get { return this.PrivRunningTimeLastUpdate; }
                set
                {
                    if (this.PrivRunningTimeLastUpdate != value)
                    {
                        this.PrivRunningTimeLastUpdate = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivProcessId = -1;
            public int ProcessId
            {
                get { return this.PrivProcessId; }
                set
                {
                    if (this.PrivProcessId != value)
                    {
                        this.PrivProcessId = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private IntPtr PrivWindowHandle = IntPtr.Zero;
            public IntPtr WindowHandle
            {
                get { return this.PrivWindowHandle; }
                set
                {
                    if (this.PrivWindowHandle != value)
                    {
                        this.PrivWindowHandle = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivProcessRunningCount = string.Empty;
            public string ProcessRunningCount
            {
                get { return this.PrivProcessRunningCount; }
                set
                {
                    if (this.PrivProcessRunningCount != value)
                    {
                        this.PrivProcessRunningCount = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivProcessName;
            public string ProcessName
            {
                get { return this.PrivProcessName; }
                set
                {
                    if (this.PrivProcessName != value)
                    {
                        this.PrivProcessName = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivRomPath;
            public string RomPath
            {
                get { return this.PrivRomPath; }
                set
                {
                    if (this.PrivRomPath != value)
                    {
                        this.PrivRomPath = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivShortcutPath;
            public string ShortcutPath
            {
                get { return this.PrivShortcutPath; }
                set
                {
                    if (this.PrivShortcutPath != value)
                    {
                        this.PrivShortcutPath = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private BitmapImage PrivImageBitmap;
            public BitmapImage ImageBitmap
            {
                get { return this.PrivImageBitmap; }
                set
                {
                    if (this.PrivImageBitmap != value)
                    {
                        this.PrivImageBitmap = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivImagePath;
            public string ImagePath
            {
                get { return this.PrivImagePath; }
                set
                {
                    if (this.PrivImagePath != value)
                    {
                        this.PrivImagePath = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private DateTime PrivTimeCreation;
            public DateTime TimeCreation
            {
                get { return this.PrivTimeCreation; }
                set
                {
                    if (this.PrivTimeCreation != value)
                    {
                        this.PrivTimeCreation = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility? PrivStatusAvailable;
            public Visibility? StatusAvailable
            {
                get
                {
                    if (this.PrivStatusAvailable == null) { return Visibility.Collapsed; }
                    else { return this.PrivStatusAvailable; }
                }
                set
                {
                    if (this.PrivStatusAvailable != value)
                    {
                        this.PrivStatusAvailable = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility? PrivStatusRunning;
            public Visibility? StatusRunning
            {
                get
                {
                    if (this.PrivStatusRunning == null) { return Visibility.Collapsed; }
                    else { return this.PrivStatusRunning; }
                }
                set
                {
                    if (this.PrivStatusRunning != value)
                    {
                        this.PrivStatusRunning = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility? PrivStatusStore;
            public Visibility? StatusStore
            {
                get
                {
                    if (this.PrivStatusStore == null) { return Visibility.Collapsed; }
                    else { return this.PrivStatusStore; }
                }
                set
                {
                    if (this.PrivStatusStore != value)
                    {
                        this.PrivStatusStore = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility? PrivStatusLauncher;
            public Visibility? StatusLauncher
            {
                get
                {
                    if (this.PrivStatusLauncher == null) { return Visibility.Collapsed; }
                    else { return this.PrivStatusLauncher; }
                }
                set
                {
                    if (this.PrivStatusLauncher != value)
                    {
                        this.PrivStatusLauncher = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}