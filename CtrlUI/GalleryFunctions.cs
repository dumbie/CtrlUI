using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Get all media and update the list
        async Task RefreshListGallery(bool showStatus)
        {
            try
            {
                //Check if application is activated
                if (!vAppActivated)
                {
                    return;
                }

                //Check if already refreshing
                if (vBusyRefreshingGallery)
                {
                    Debug.WriteLine("Gallery is already refreshing, cancelling.");
                    return;
                }

                //Check last update time
                long updateTime = GetSystemTicksMs();
                long updateOffset = updateTime - vLastUpdateGallery;
                if (updateOffset < 30000)
                {
                    //Debug.WriteLine("Gallery recently refreshed, cancelling.");
                    return;
                }

                //Update refreshing status
                vLastUpdateGallery = updateTime;
                vBusyRefreshingGallery = true;

                //Show the loading gif
                AVActions.DispatcherInvoke(delegate
                {
                    gif_List_Loading.Show();
                });

                //Check if loading first time
                bool firstLoad = !List_Gallery.Any();

                //Show refresh status message
                if (showStatus)
                {
                    Notification_Show_Status("Refresh", "Refreshing gallery");
                }

                //Get gallery load days setting
                int galleryLoadDaysInt = SettingLoad(vConfigurationCtrlUI, "GalleryLoadDays", typeof(int));
                DateTime galleryLoadDaysDateTime = DateTime.Now.AddDays(-galleryLoadDaysInt);

                //Get all files from gallery directories
                IEnumerable<FileInfo> directoryGallery = Enumerable.Empty<FileInfo>();
                foreach (ProfileShared galleryFolder in vCtrlLocationsGallery)
                {
                    try
                    {
                        string editedGalleryFolder = ConvertEnvironmentPath(galleryFolder.String1);
                        if (Directory.Exists(editedGalleryFolder))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(editedGalleryFolder);
                            IEnumerable<FileInfo> filterGallery = directoryInfo.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Name.ToLower().EndsWith(".jpg") || x.Name.ToLower().EndsWith(".jxr") || x.Name.ToLower().EndsWith(".png") || x.Name.ToLower().EndsWith(".gif") || x.Name.ToLower().EndsWith(".mp4")).Where(x => x.LastWriteTime >= galleryLoadDaysDateTime);
                            directoryGallery = directoryGallery.Concat(filterGallery);
                        }
                    }
                    catch { }
                }

                //Sort and filter list by creation time
                directoryGallery = directoryGallery.OrderBy(x => x.CreationTime);

                //Remove media that is no longer available from the list
                Func<DataBindApp, bool> filterGalleryApp = x => x.Category == AppCategory.Gallery && !directoryGallery.Any(y => y.FullName == x.PathGallery);
                await ListBoxRemoveAll(lb_Gallery, List_Gallery, filterGalleryApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterGalleryApp);

                //Get media information and add it to the list
                foreach (FileInfo file in directoryGallery)
                {
                    try
                    {
                        //Get media details
                        string mediaName = Path.GetFileNameWithoutExtension(file.Name);
                        string mediaExtension = Path.GetExtension(file.Name);

                        //Get media path
                        string mediaPath = file.FullName;

                        //Check if media is already in gallery list
                        Func<DataBindApp, bool> duplicateCheck = x => (x.PathGallery == mediaPath);
                        DataBindApp mediaExistCheck = List_Gallery.FirstOrDefault(duplicateCheck);
                        if (mediaExistCheck != null)
                        {
                            //Debug.WriteLine("Media is already in list, skipping: " + mediaPath);
                            continue;
                        }

                        //Check if media is video
                        bool mediaVideo = mediaExtension == ".mp4" || mediaExtension == ".gif";
                        Visibility statusVideo = mediaVideo ? Visibility.Visible : Visibility.Collapsed;

                        //Add media to gallery list
                        DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Unknown, Category = AppCategory.Gallery, Name = mediaName, PathGallery = mediaPath, StatusVideo = statusVideo, DateModified = file.LastWriteTime };
                        await ListBoxAddItem(lb_Gallery, List_Gallery, dataBindApp, true, false);
                    }
                    catch { }
                }

                //First load functions
                if (firstLoad)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        lb_Gallery.SelectedIndex = 0;
                    });
                }

                //Hide the loading gif
                if (vBusyRefreshingCount() == 1)
                {
                    AVActions.DispatcherInvoke(delegate
                    {
                        gif_List_Loading.Hide();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading gallery: " + ex.Message);
            }
            finally
            {
                //Update list load status
                vListLoadedGallery = true;

                //Update the refreshing status
                vBusyRefreshingGallery = false;
            }
        }
    }
}