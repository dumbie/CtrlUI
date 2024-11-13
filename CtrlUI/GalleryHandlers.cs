using ArnoldVinkCode;
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
        private void ListBox_GalleryScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                AVFunctions.TimerRenew(ref vDispatcherTimerDelay);
                vDispatcherTimerDelay.Interval = TimeSpan.FromMilliseconds(50);
                vDispatcherTimerDelay.Tick += delegate
                {
                    AVFunctions.TimerStop(vDispatcherTimerDelay);
                    UpdateGalleryMediaImages();
                };
                AVFunctions.TimerReset(vDispatcherTimerDelay);
            }
            catch { }
        }

        private void UpdateGalleryMediaImages()
        {
            try
            {
                foreach (DataBindApp dataBindApp in lb_Gallery.Items)
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