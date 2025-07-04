using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using static LibraryShared.Enums;

namespace LibraryShared
{
    public partial class Classes
    {
        public class DataBindFile : INotifyPropertyChanged
        {
            private FileType PrivFileType;
            public FileType FileType
            {
                get { return this.PrivFileType; }
                set
                {
                    if (this.PrivFileType != value)
                    {
                        this.PrivFileType = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private ClipboardType PrivClipboardType;
            public ClipboardType ClipboardType
            {
                get { return this.PrivClipboardType; }
                set
                {
                    if (this.PrivClipboardType != value)
                    {
                        this.PrivClipboardType = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private Visibility PrivChecked = Visibility.Collapsed;
            public Visibility Checked
            {
                get { return this.PrivChecked; }
                set
                {
                    if (this.PrivChecked != value)
                    {
                        this.PrivChecked = value;
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

            private DateTime PrivDateModified;
            public DateTime DateModified
            {
                get { return this.PrivDateModified; }
                set
                {
                    if (this.PrivDateModified != value)
                    {
                        this.PrivDateModified = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivIsShortcut;
            public bool IsShortcut
            {
                get { return this.PrivIsShortcut; }
                set
                {
                    if (this.PrivIsShortcut != value)
                    {
                        this.PrivIsShortcut = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivPathFile;
            public string PathFile
            {
                get { return this.PrivPathFile; }
                set
                {
                    if (this.PrivPathFile != value)
                    {
                        this.PrivPathFile = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivPathFull;
            public string PathFull
            {
                get { return this.PrivPathFull; }
                set
                {
                    if (this.PrivPathFull != value)
                    {
                        this.PrivPathFull = value;
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

            private string PrivPathRoot;
            public string PathRoot
            {
                get { return this.PrivPathRoot; }
                set
                {
                    if (this.PrivPathRoot != value)
                    {
                        this.PrivPathRoot = value;
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

            private string PrivNameSub;
            public string NameSub
            {
                get { return this.PrivNameSub; }
                set
                {
                    if (this.PrivNameSub != value)
                    {
                        this.PrivNameSub = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivNameDetail;
            public string NameDetail
            {
                get { return this.PrivNameDetail; }
                set
                {
                    if (this.PrivNameDetail != value)
                    {
                        this.PrivNameDetail = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivDescription;
            public string Description
            {
                get { return this.PrivDescription; }
                set
                {
                    if (this.PrivDescription != value)
                    {
                        this.PrivDescription = value;
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