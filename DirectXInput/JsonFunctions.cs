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
        void JsonLoadList_ControllerProfile()
        {
            try
            {
                //Remove all the current controllers
                vDirectControllersProfile.Clear();

                string JsonFile = File.ReadAllText(@"Profiles\DirectControllersProfile.json");
                ControllerProfile[] JsonList = JsonConvert.DeserializeObject<ControllerProfile[]>(JsonFile);
                foreach (ControllerProfile Controller in JsonList)
                {
                    try
                    {
                        vDirectControllersProfile.Add(Controller);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Controller Profile Json completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json: " + ex.Message);
            }
        }

        //Read from Json file (Deserialize)
        void JsonLoadList_ControllerSupported()
        {
            try
            {
                //Remove all the current controllers
                vDirectControllersSupported.Clear();

                string JsonFile = File.ReadAllText(@"Profiles\DirectControllersSupported.json");
                ControllerSupported[] JsonList = JsonConvert.DeserializeObject<ControllerSupported[]>(JsonFile);
                foreach (ControllerSupported Controller in JsonList)
                {
                    try
                    {
                        vDirectControllersSupported.Add(Controller);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Controller Supported Json completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json: " + ex.Message);
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
    }
}