using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVJsonFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Read apps from Json file (Deserialize)
        async Task JsonLoadList_Applications()
        {
            try
            {
                //Remove all the current apps
                List_Games.Clear();
                List_Apps.Clear();
                List_Emulators.Clear();

                //Add all the apps to the list
                string JsonFile = File.ReadAllText(@"Profiles\User\CtrlApplications.json");
                DataBindApp[] JsonList = JsonConvert.DeserializeObject<DataBindApp[]>(JsonFile).OrderBy(x => x.Number).ToArray();
                foreach (DataBindApp dataBindApp in JsonList)
                {
                    try
                    {
                        await AddAppToList(dataBindApp, false, true);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Json applications completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading Json applications: " + ex.Message);
            }
            finally
            {
                //Update list load status
                vListLoadedApplications = true;
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveList_Applications()
        {
            try
            {
                //Combine applications
                var JsonFilterList = CombineAppLists(true, true, true, false, false, false).Select(x => new { x.Number, x.Category, x.Type, x.Name, x.AppUserModelId, x.NameExe, x.PathExe, x.PathLaunch, x.PathRoms, x.Argument, x.QuickLaunch, x.LaunchAsAdmin, x.LaunchFilePicker, x.LaunchSkipRom, x.LaunchKeyboard, x.LaunchEnableDisplayHDR, x.LaunchEnableAutoHDR, x.LastLaunch, x.RunningTime, x.EmulatorName, x.EmulatorCategory, x.LightImageBackground });

                //Save object to json
                JsonSaveObject(JsonFilterList, @"Profiles\User\CtrlApplications.json");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json apps: " + ex.Message);
            }
        }
    }
}