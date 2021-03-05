using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                string JsonFile = File.ReadAllText(@"Profiles\CtrlApplications.json");
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
        }

        //Save to Json file (Serialize)
        void JsonSaveApplications()
        {
            try
            {
                //Combine apps
                var JsonFilterList = CombineAppLists(false, false, false).Select(x => new { x.Number, x.Category, x.Type, x.Name, x.NameExe, x.PathImage, x.PathExe, x.PathLaunch, x.PathRoms, x.Argument, x.QuickLaunch, x.LaunchFilePicker, x.LaunchKeyboard, x.LastLaunch, x.RunningTime });

                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedList = JsonConvert.SerializeObject(JsonFilterList, jsonSettings);

                //Save to file
                File.WriteAllText(@"Profiles\CtrlApplications.json", serializedList);
                Debug.WriteLine("Saving Json apps completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json apps: " + ex.Message);
            }
        }
    }
}