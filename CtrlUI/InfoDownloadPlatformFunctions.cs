using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Search platform information
        public async Task<ApiIGDBPlatforms> Popup_SearchInfoPlatform(string searchTerm, string searchSubtitle, string searchDescription)
        {
            try
            {
                //Show the text input popup
                string nameDownload = await Popup_ShowHide_TextInput("Platform search", searchTerm, "Search", true);
                if (string.IsNullOrWhiteSpace(nameDownload))
                {
                    Debug.WriteLine("No search term entered.");
                    return null;
                }
                nameDownload = nameDownload.ToLower();

                await Notification_Send_Status("Download", "Downloading information");
                Debug.WriteLine("Downloading information for: " + searchTerm);

                //Download available games
                ApiIGDBPlatforms[] iGDBPlatforms = await ApiIGDB_DownloadPlatforms_Search(nameDownload);
                if (iGDBPlatforms == null || !iGDBPlatforms.Any())
                {
                    Debug.WriteLine("No platforms found for: " + searchTerm);
                    await Notification_Send_Status("Close", "No platforms found");
                    return null;
                }

                //Return only result
                if (iGDBPlatforms.Count() == 1)
                {
                    Debug.WriteLine("Only one platforms found.");
                    return iGDBPlatforms.FirstOrDefault();
                }

                //Ask user which platform to download
                List<DataBindString> Answers = new List<DataBindString>();
                foreach (ApiIGDBPlatforms infoPlatforms in iGDBPlatforms)
                {
                    try
                    {
                        //Get first versions
                        ApiIGDBPlatformsVersions infoVersions = infoPlatforms.versions.FirstOrDefault();

                        //Get release year
                        string releaseYear = string.Empty;
                        if (infoVersions != null)
                        {
                            ApiIGDBReleaseDates infoReleaseDate = infoVersions.platform_version_release_dates.FirstOrDefault();
                            if (infoReleaseDate != null)
                            {
                                releaseYear = infoReleaseDate.y.ToString();
                            }
                        }

                        DataBindString answerDownload = new DataBindString();
                        answerDownload.ImageBitmap = vImagePreloadEmulator;
                        answerDownload.Name = infoPlatforms.name;
                        answerDownload.NameSub = infoPlatforms.alternative_name;
                        answerDownload.NameDetail = releaseYear;
                        answerDownload.Data1 = infoPlatforms;
                        Answers.Add(answerDownload);
                    }
                    catch { }
                }

                //Get selected result
                DataBindString messageResult = await Popup_Show_MessageBox("Select a found platform (" + Answers.Count() + ")", searchSubtitle, searchDescription, Answers);
                if (messageResult == null)
                {
                    Debug.WriteLine("No platform selected");
                    return null;
                }

                //Convert result back to json
                return (ApiIGDBPlatforms)messageResult.Data1;
            }
            catch
            {
                return null;
            }
        }
    }
}