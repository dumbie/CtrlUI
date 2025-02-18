using ArnoldVinkCode;
using System;
using System.Windows.Controls;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVInterface;
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
                AVFunctions.TimerRenew(ref vDispatcherTimerDelay);
                vDispatcherTimerDelay.Interval = TimeSpan.FromMilliseconds(50);
                vDispatcherTimerDelay.Tick += delegate
                {
                    AVFunctions.TimerStop(vDispatcherTimerDelay);
                    UpdateGalleryMediaImages(false);
                };
                AVFunctions.TimerReset(vDispatcherTimerDelay);
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