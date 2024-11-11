using ArnoldVinkCode;
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
                //Fix high cpu load when scrolling very fast
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
                                void TaskAction()
                                {
                                    dataBindApp.ImageBitmap = FileCacheToBitmapImage(dataBindApp.PathGallery, vImageBackupSource, 200, 0, false);
                                }
                                AVActions.TaskStartBackground(TaskAction);
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