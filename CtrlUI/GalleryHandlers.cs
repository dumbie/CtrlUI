using ArnoldVinkCode;
using System.Windows.Controls;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static ArnoldVinkStyles.AVInterface;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        private void ListBox_GalleryScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                //Start delay timer
                vAVTimerDelay.Interval = 50;
                vAVTimerDelay.Tick = delegate
                {
                    try
                    {
                        DispatcherInvoke(delegate
                        {
                            //Stop delay timer
                            vAVTimerDelay.Stop();

                            //Update gallery images
                            UpdateGalleryMediaImages(false);
                        });
                    }
                    catch { }
                };
                vAVTimerDelay.Start();
            }
            catch { }
        }

        private void UpdateGalleryMediaImages(bool searchListBox)
        {
            try
            {
                ListBox targetListBox = searchListBox ? lb_Search : lb_Gallery;
                foreach (DataBindApp dataBindApp in targetListBox.Items)
                {
                    try
                    {
                        if (dataBindApp.Category != AppCategory.Gallery) { continue; }
                        ListBoxItem listBoxItem = (ListBoxItem)targetListBox.ItemContainerGenerator.ContainerFromItem(dataBindApp);
                        if (FrameworkElementVisibleUser(listBoxItem, this))
                        {
                            if (dataBindApp.ImageBitmap == null)
                            {
                                void TaskAction()
                                {
                                    dataBindApp.ImageBitmap = FileCacheToBitmapImage(dataBindApp.PathGallery, vImageBackupSource, 384, 0, false);
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