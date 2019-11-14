using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Read apps from Json file (Deserialize)
        void JsonLoadApps()
        {
            try
            {
                //Remove all the current apps
                List_Games.Clear();
                List_Apps.Clear();
                List_Emulators.Clear();
                GC.Collect();

                //Add all the apps to the list
                string JsonFile = File.ReadAllText(@"Profiles\Apps.json");
                DataBindApp[] JsonList = JsonConvert.DeserializeObject<DataBindApp[]>(JsonFile).OrderBy(x => x.Number).ToArray();
                foreach (DataBindApp App in JsonList)
                {
                    try
                    {
                        AddAppToList(App, false, true);
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

        //Read other launchers from Json file (Deserialize)
        void JsonLoadAppsOtherLaunchers()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\AppsOtherLaunchers.json");
                vAppsOtherLaunchers = JsonConvert.DeserializeObject<string[]>(JsonFile);

                Debug.WriteLine("Reading Json other launchers completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json other launchers: " + ex.Message);
            }
        }

        //Read file locations from Json file (Deserialize)
        void JsonLoadFileLocations()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\FileLocations.json");
                vFileLocations = JsonConvert.DeserializeObject<List<FileLocation>>(JsonFile);

                Debug.WriteLine("Reading Json file locations completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json file locations: " + ex.Message);
            }
        }

        //Read blacklist shortcut from Json file (Deserialize)
        void JsonLoadAppsBlacklistShortcut()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\AppsBlacklistShortcut.json");
                vAppsBlacklistShortcut = JsonConvert.DeserializeObject<List<string>>(JsonFile);

                Debug.WriteLine("Reading Json blacklist shortcuts completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json blacklist shortcut: " + ex.Message);
            }
        }

        //Read blacklist shortcut uri from Json file (Deserialize)
        void JsonLoadAppsBlacklistShortcutUri()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\AppsBlacklistShortcutUri.json");
                vAppsBlacklistShortcutUri = JsonConvert.DeserializeObject<string[]>(JsonFile);

                Debug.WriteLine("Reading Json blacklist shortcuts uri completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json blacklist shortcut uri: " + ex.Message);
            }
        }

        //Read blacklist process from Json file (Deserialize)
        void JsonLoadAppsBlacklistProcess()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\AppsBlacklistProcess.json");
                vAppsBlacklistProcess = JsonConvert.DeserializeObject<string[]>(JsonFile);

                Debug.WriteLine("Reading Json blacklist process completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json blacklist process: " + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveAppsBlacklistShortcut()
        {
            try
            {
                string SerializedList = JsonConvert.SerializeObject(vAppsBlacklistShortcut);
                File.WriteAllText(@"Profiles\AppsBlacklistShortcut.json", SerializedList);

                Debug.WriteLine("Saving Json shortcut blacklist completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json shortcut blacklist: " + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveApps()
        {
            try
            {
                var JsonFilterList = CombineAppLists(false, false).Select(x => new { x.Number, x.Category, x.Type, x.Name, x.PathImage, x.PathExe, x.PathLaunch, x.PathRoms, x.Argument, x.QuickLaunch, x.FilePickerLaunch, x.RunningTime });
                string SerializedList = JsonConvert.SerializeObject(JsonFilterList);
                File.WriteAllText(@"Profiles\Apps.json", SerializedList);

                Debug.WriteLine("Saving Json apps completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json apps: " + ex.Message);
            }
        }
    }
}