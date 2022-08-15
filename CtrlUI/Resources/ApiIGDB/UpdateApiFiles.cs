using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update IGDB api files
        public async Task ApiIGDB_UpdateFiles()
        {
            try
            {
                Debug.WriteLine("Checking IGDB api files.");

                //Load last api update time
                string lastUpdateString = Setting_Load(vConfigurationCtrlUI, "ApiIGDBUpdate").ToString();
                DateTime lastUpdateDateTime = DateTime.Parse(lastUpdateString, vAppCultureInfo);

                //Check if days have passed
                double daysPassed = DateTime.Now.Subtract(lastUpdateDateTime).TotalDays;
                if (daysPassed > 7)
                {
                    bool genresUpdated = await ApiIGDBDownloadGenres();
                    bool platformsUpdated = await ApiIGDBDownloadPlatforms();
                    if (genresUpdated && platformsUpdated)
                    {
                        Setting_Save(vConfigurationCtrlUI, "ApiIGDBUpdate", DateTime.Now.ToString(vAppCultureInfo));
                        Debug.WriteLine("Updated IGDB api files.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed updating IGDB api files: " + ex.Message);
            }
        }
    }
}