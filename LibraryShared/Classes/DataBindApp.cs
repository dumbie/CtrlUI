using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVProcess;
using static LibraryShared.Enums;

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

            private AppCategory PrivCategory = AppCategory.Unknown;
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

            private AppLauncher PrivLauncher = AppLauncher.Unknown;
            public AppLauncher Launcher
            {
                get { return this.PrivLauncher; }
                set
                {
                    if (this.PrivLauncher != value)
                    {
                        this.PrivLauncher = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private ProcessType PrivType = ProcessType.Unknown;
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

            private string PrivName = string.Empty;
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

            private string PrivAppUserModelId = string.Empty;
            public string AppUserModelId
            {
                get { return this.PrivAppUserModelId; }
                set
                {
                    if (this.PrivAppUserModelId != value)
                    {
                        this.PrivAppUserModelId = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivNameExe = string.Empty;
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

            private string PrivPathExe = string.Empty;
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

            private string PrivPathLaunch = string.Empty;
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

            private string PrivPathRoms = string.Empty;
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

            private string PrivArgument = string.Empty;
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

            private bool PrivLaunchFilePicker = false;
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

            private bool PrivLaunchSkipRom = false;
            public bool LaunchSkipRom
            {
                get { return this.PrivLaunchSkipRom; }
                set
                {
                    if (this.PrivLaunchSkipRom != value)
                    {
                        this.PrivLaunchSkipRom = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivLaunchKeyboard = false;
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

            private bool PrivLaunchEnableHDR = false;
            public bool LaunchEnableHDR
            {
                get { return this.PrivLaunchEnableHDR; }
                set
                {
                    if (this.PrivLaunchEnableHDR != value)
                    {
                        this.PrivLaunchEnableHDR = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivQuickLaunch = false;
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

            private string PrivLastLaunch = string.Empty;
            public string LastLaunch
            {
                get { return this.PrivLastLaunch; }
                set
                {
                    if (this.PrivLastLaunch != value)
                    {
                        this.PrivLastLaunch = value;
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

            //Emulator variables
            private string PrivEmulatorName = string.Empty;
            public string EmulatorName
            {
                get { return this.PrivEmulatorName; }
                set
                {
                    if (this.PrivEmulatorName != value)
                    {
                        this.PrivEmulatorName = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private EmulatorCategory PrivEmulatorCategory = EmulatorCategory.Other;
            public EmulatorCategory EmulatorCategory
            {
                get { return this.PrivEmulatorCategory; }
                set
                {
                    if (this.PrivEmulatorCategory != value)
                    {
                        this.PrivEmulatorCategory = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Status variables (no saving needed)
            private List<ProcessMulti> PrivProcessMulti = new List<ProcessMulti>();
            public List<ProcessMulti> ProcessMulti
            {
                get { return this.PrivProcessMulti; }
                set
                {
                    if (this.PrivProcessMulti != value)
                    {
                        this.PrivProcessMulti = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivStatusProcessCount = string.Empty;
            public string StatusProcessCount
            {
                get { return this.PrivStatusProcessCount; }
                set
                {
                    if (this.PrivStatusProcessCount != value)
                    {
                        this.PrivStatusProcessCount = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivStatusProcessRunTime = -1;
            public int StatusProcessRunTime
            {
                get { return this.PrivStatusProcessRunTime; }
                set
                {
                    if (this.PrivStatusProcessRunTime != value)
                    {
                        this.PrivStatusProcessRunTime = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivShortcutPath = string.Empty;
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

            private BitmapImage PrivImageBitmap = null;
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

            private DateTime PrivTimeCreation = new DateTime();
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

            private Visibility PrivStatusAvailable = Visibility.Collapsed;
            public Visibility StatusAvailable
            {
                get { return this.PrivStatusAvailable; }
                set
                {
                    if (this.PrivStatusAvailable != value)
                    {
                        this.PrivStatusAvailable = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivStatusRunning = Visibility.Collapsed;
            public Visibility StatusRunning
            {
                get { return this.PrivStatusRunning; }
                set
                {
                    if (this.PrivStatusRunning != value)
                    {
                        this.PrivStatusRunning = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivStatusSuspended = Visibility.Collapsed;
            public Visibility StatusSuspended
            {
                get { return this.PrivStatusSuspended; }
                set
                {
                    if (this.PrivStatusSuspended != value)
                    {
                        this.PrivStatusSuspended = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivStatusNotResponding = Visibility.Collapsed;
            public Visibility StatusNotResponding
            {
                get { return this.PrivStatusNotResponding; }
                set
                {
                    if (this.PrivStatusNotResponding != value)
                    {
                        this.PrivStatusNotResponding = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivStatusStore = Visibility.Collapsed;
            public Visibility StatusStore
            {
                get { return this.PrivStatusStore; }
                set
                {
                    if (this.PrivStatusStore != value)
                    {
                        this.PrivStatusStore = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivStatusUrlProtocol = Visibility.Collapsed;
            public Visibility StatusUrlProtocol
            {
                get { return this.PrivStatusUrlProtocol; }
                set
                {
                    if (this.PrivStatusUrlProtocol != value)
                    {
                        this.PrivStatusUrlProtocol = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivStatusUrlBrowser = Visibility.Collapsed;
            public Visibility StatusUrlBrowser
            {
                get { return this.PrivStatusUrlBrowser; }
                set
                {
                    if (this.PrivStatusUrlBrowser != value)
                    {
                        this.PrivStatusUrlBrowser = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private BitmapImage PrivStatusEmulatorCategoryImage = null;
            public BitmapImage StatusEmulatorCategoryImage
            {
                get { return this.PrivStatusEmulatorCategoryImage; }
                set
                {
                    if (this.PrivStatusEmulatorCategoryImage != value)
                    {
                        this.PrivStatusEmulatorCategoryImage = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private BitmapImage PrivStatusSearchCategoryImage = null;
            public BitmapImage StatusSearchCategoryImage
            {
                get { return this.PrivStatusSearchCategoryImage; }
                set
                {
                    if (this.PrivStatusSearchCategoryImage != value)
                    {
                        this.PrivStatusSearchCategoryImage = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private BitmapImage PrivStatusLauncherImage = null;
            public BitmapImage StatusLauncherImage
            {
                get { return this.PrivStatusLauncherImage; }
                set
                {
                    if (this.PrivStatusLauncherImage != value)
                    {
                        this.PrivStatusLauncherImage = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Reset application status
            public void ResetStatus(bool resetAvailable)
            {
                try
                {
                    if (resetAvailable)
                    {
                        StatusAvailable = Visibility.Collapsed;
                    }

                    StatusRunning = Visibility.Collapsed;
                    StatusSuspended = Visibility.Collapsed;
                    StatusNotResponding = Visibility.Collapsed;
                    StatusProcessCount = string.Empty;
                    ProcessMulti.Clear();

                    //Debug.WriteLine("Reset application status: " + Name + "/" + Category);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to reset application status: " + Name + "/" + ex.Message);
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