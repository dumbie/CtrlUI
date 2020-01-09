using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Read from Json file (Deserialize)
        void JsonLoadControllerProfile()
        {
            try
            {
                //Remove all the current controllers
                List_ControllerProfile.Clear();

                string JsonFile = File.ReadAllText(@"Profiles\Controllers.json");
                ControllerProfile[] JsonList = JsonConvert.DeserializeObject<ControllerProfile[]>(JsonFile);
                foreach (ControllerProfile Controller in JsonList)
                {
                    try
                    {
                        List_ControllerProfile.Add(Controller);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Controller Profile Json completed.");
            }
            catch (Exception ex) { Debug.WriteLine("Failed Reading Json: " + ex.Message); }
        }

        //Read other tools from Json file (Deserialize)
        void JsonLoadAppsCloseTools()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\AppsCloseTools.json");
                vAppsCloseTools = JsonConvert.DeserializeObject<string[]>(JsonFile);

                Debug.WriteLine("Reading Json other tools completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json other tools: " + ex.Message);
            }
        }

        //Read from Json file (Deserialize)
        void JsonLoadList_ControllerSupported()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\ControllersSupported.json");
                ControllerSupported[] JsonList = JsonConvert.DeserializeObject<ControllerSupported[]>(JsonFile);
                foreach (ControllerSupported Controller in JsonList)
                {
                    try
                    {
                        List_ControllerSupported.Add(Controller);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Controller Supported Json completed.");
            }
            catch (Exception ex) { Debug.WriteLine("Failed Reading Json: " + ex.Message); }
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
    }
}