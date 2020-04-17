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

            private object PrivData1;
            public object Data1
            {
                get { return this.PrivData1; }
                set
                {
                    if (this.PrivData1 != value)
                    {
                        this.PrivData1 = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private object PrivData2;
            public object Data2
            {
                get { return this.PrivData2; }
                set
                {
                    if (this.PrivData2 != value)
                    {
                        this.PrivData2 = value;
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