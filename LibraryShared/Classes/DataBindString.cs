using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace LibraryShared
{
    public partial class Classes
    {
        public class DataBindString : INotifyPropertyChanged
        {
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

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}