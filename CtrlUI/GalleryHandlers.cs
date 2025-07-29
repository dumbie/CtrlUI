using ArnoldVinkCode;
using ArnoldVinkStyles;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkStyles.AVDispatcherInvoke;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        private void ListView_GalleryScrollViewer_ScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            try
            {
                //Start delay timer
                vAVTimerDelayGallery.Interval = 50;
                vAVTimerDelayGallery.Tick = delegate
                {
                    //Stop delay timer
                    vAVTimerDelayGallery.Stop();

                    //Update gallery images
                    UpdateGalleryMediaImages(false);
                };
                vAVTimerDelayGallery.Start();
            }
            catch { }
        }

        private void UpdateGalleryMediaImages(bool searchListView)
        {
            try
            {
                DispatcherInvoke(this.Dispatcher, delegate
                {
                    ListView targetListView = searchListView ? listView_Search : listView_Gallery;
                    foreach (DataBindApp dataBindApp in targetListView.Items)
                    {
                        try
                        {
                            if (dataBindApp.Category != AppCategory.Gallery) { continue; }
                            ListViewItem listBoxItem = targetListView.AVGetListViewItem(dataBindApp);
                            if (listBoxItem.AVVisibleUser(this))
                            {
                                if (dataBindApp.ImageBitmap == null)
                                {
                                    async void TaskAction()
                                    {
                                        await DispatcherInvoke(this.Dispatcher, async delegate
                                        {
                                            dataBindApp.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                                            {
                                                FilePaths = [dataBindApp.PathGallery],
                                                BackupPath = vImageBackupSource,
                                                Width = vImageLoadSizeGallery,
                                                UseThumbnail = true,
                                                Dispatcher = this.Dispatcher
                                            });
                                        });
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
                });
            }
            catch { }
        }
    }
}