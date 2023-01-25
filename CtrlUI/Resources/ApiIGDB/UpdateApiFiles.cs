using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;

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
                string lastUpdateString = SettingLoad(vConfigurationCtrlUI, "ApiIGDBUpdate", typeof(string));
                DateTime lastUpdateDateTime = DateTime.Parse(lastUpdateString, vAppCultureInfo);

                //Check if days have passed
                double daysPassed = DateTime.Now.Subtract(lastUpdateDateTime).TotalDays;
                if (daysPassed > 7)
                {
                    bool genresUpdated = await ApiIGDB_DownloadGenres();
                    bool platformsUpdated = await ApiIGDB_DownloadPlatforms();
                    if (genresUpdated && platformsUpdated)
                    {
                        SettingSave(vConfigurationCtrlUI, "ApiIGDBUpdate", DateTime.Now.ToString(vAppCultureInfo));
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