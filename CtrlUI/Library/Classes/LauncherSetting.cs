using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using static LibraryShared.Enums;

namespace LibraryShared
{
    public partial class Classes
    {
        public class LauncherSetting : INotifyPropertyChanged
        {
            private AppLauncher PrivAppLauncher;
            public AppLauncher AppLauncher
            {
                get { return this.PrivAppLauncher; }
                set
                {
                    if (this.PrivAppLauncher != value)
                    {
                        this.PrivAppLauncher = value;
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

            private bool PrivEnabled;
            public bool Enabled
            {
                get { return this.PrivEnabled; }
                set
                {
                    if (this.PrivEnabled != value)
                    {
                        this.PrivEnabled = value;
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