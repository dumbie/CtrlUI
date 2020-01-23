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

                Debug.WriteLine("Reading Json apps completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading Json apps: " + ex.Message);
            }
        }

        //Read Json from profile (Deserialize)
        void JsonLoadProfile<T>(ref T deserializeTarget, string profileName)
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\" + profileName + ".json");
                deserializeTarget = JsonConvert.DeserializeObject<T>(JsonFile);
                Debug.WriteLine("Reading Json file completed: " + profileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Reading Json file failed: " + profileName + "/" + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveObject(object serializeObject, string profileName)
        {
            try
            {
                string serializedObject = JsonConvert.SerializeObject(serializeObject);
                File.WriteAllText(@"Profiles\" + profileName + ".json", serializedObject);
                Debug.WriteLine("Saving Json " + profileName + " completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json " + profileName + ": " + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveApplications()
        {
            try
            {
                var JsonFilterList = CombineAppLists(false, false).Select(x => new { x.Number, x.Category, x.Type, x.Name, x.NameExe, x.PathImage, x.PathExe, x.PathLaunch, x.PathRoms, x.Argument, x.QuickLaunch, x.LaunchFilePicker, x.LaunchKeyboard, x.RunningTime });
                string SerializedList = JsonConvert.SerializeObject(JsonFilterList);
                File.WriteAllText(@"Profiles\CtrlApplications.json", SerializedList);
                Debug.WriteLine("Saving Json apps completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json apps: " + ex.Message);
            }
        }
    }
}