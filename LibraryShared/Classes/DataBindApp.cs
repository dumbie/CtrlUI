using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.ProcessClasses;
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

            private string PrivPathImage = string.Empty;
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

            private int PrivRunningTime = -2;
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

            private string PrivRunningProcessCount = string.Empty;
            public string RunningProcessCount
            {
                get { return this.PrivRunningProcessCount; }
                set
                {
                    if (this.PrivRunningProcessCount != value)
                    {
                        this.PrivRunningProcessCount = value;
                        NotifyPropertyChanged();
                    }
                }
            }

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

            private BitmapImage PrivImageBitmap = new BitmapImage();
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

            private Visibility PrivStatusLauncher = Visibility.Collapsed;
            public Visibility StatusLauncher
            {
                get { return this.PrivStatusLauncher; }
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