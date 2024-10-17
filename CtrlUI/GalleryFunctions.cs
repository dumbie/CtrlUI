using ArnoldVinkCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVProcess;
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

                //Show refresh status message
                if (showStatus)
                {
                    await Notification_Send_Status("Refresh", "Refreshing gallery");
                }

                //Get all files from gallery directories
                IEnumerable<FileInfo> directoryGallery = Enumerable.Empty<FileInfo>();
                foreach (ProfileShared galleryFolder in vCtrlLocationsGallery)
                {
                    try
                    {
                        string editedGalleryFolder = galleryFolder.String1;
                        editedGalleryFolder = editedGalleryFolder.Replace("%DESKTOPUSER%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                        editedGalleryFolder = editedGalleryFolder.Replace("%DESKTOPPUBLIC%", Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
                        if (Directory.Exists(editedGalleryFolder))
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(editedGalleryFolder);
                            IEnumerable<FileInfo> filterGallery = directoryInfo.GetFiles("*", SearchOption.AllDirectories).Where(x => x.Name.ToLower().EndsWith(".jpg") || x.Name.ToLower().EndsWith(".jxr") || x.Name.ToLower().EndsWith(".png") || x.Name.ToLower().EndsWith(".gif") || x.Name.ToLower().EndsWith(".mp4"));
                            directoryGallery = directoryGallery.Concat(filterGallery);
                        }
                    }
                    catch { }
                }

                //Sort and filter the list by name
                directoryGallery = directoryGallery.OrderBy(x => x.Name);

                //Remove media that is no longer available from the list
                Func<DataBindApp, bool> filterGalleryApp = x => x.Category == AppCategory.Gallery && !directoryGallery.Any(y => y.FullName == x.PathExe);
                await ListBoxRemoveAll(lb_Gallery, List_Gallery, filterGalleryApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterGalleryApp);

                //Get media information and add it to the list
                foreach (FileInfo file in directoryGallery)
                {
                    try
                    {
                        //Get media name
                        string mediaName = Path.GetFileNameWithoutExtension(file.Name);

                        //Get media path
                        string mediaPath = file.FullName;

                        //Check if media is already in gallery list
                        Func<DataBindApp, bool> duplicateCheck = x => (x.PathExe == mediaPath);
                        DataBindApp mediaExistCheck = List_Gallery.FirstOrDefault(duplicateCheck);
                        if (mediaExistCheck != null)
                        {
                            //Debug.WriteLine("Media is already in list, skipping: " + mediaPath);
                            continue;
                        }

                        //Add media to gallery list
                        DataBindApp dataBindApp = new DataBindApp() { Type = ProcessType.Unknown, Category = AppCategory.Gallery, Name = mediaName, PathExe = mediaPath };
                        await ListBoxAddItem(lb_Gallery, List_Gallery, dataBindApp, true, false);
                    }
                    catch { }
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