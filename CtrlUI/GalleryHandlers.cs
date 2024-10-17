using System;
using System.Windows.Controls;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        private void LbGalleryScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                //Fix load media in separate thread to prevent freezes
                //Fix read thumbnails from windows thumbs.db to speed up
                ListBox senderListBox = (ListBox)sender;
                foreach (DataBindApp dataBindApp in senderListBox.Items)
                {
                    try
                    {
                        ListBoxItem listBoxItem = (ListBoxItem)lb_Gallery.ItemContainerGenerator.ContainerFromItem(dataBindApp);
                        if (FrameworkElementVisibleUser(listBoxItem, this))
                        {
                            if (dataBindApp.ImageBitmap == null)
                            {
                                dataBindApp.ImageBitmap = FileToBitmapImage(new string[] { dataBindApp.PathExe }, null, vImageBackupSource, IntPtr.Zero, 200, 0);
                            }
                        }
                        else
                        {
                            if (dataBindApp.ImageBitmap != null)
                            {
                                dataBindApp.ImageBitmap = null;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}