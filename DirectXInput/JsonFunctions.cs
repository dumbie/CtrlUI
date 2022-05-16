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
        void JsonLoadList_ControllerSupported()
        {
            try
            {
                //Remove all the current controllers
                vDirectControllersSupported.Clear();

                //Add all the supported controllers
                string[] jsonFiles = Directory.GetFiles(@"Profiles\Default\DirectControllersSupported", "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    string jsonFileText = File.ReadAllText(jsonFile);
                    vDirectControllersSupported.Add(JsonConvert.DeserializeObject<ControllerSupported>(jsonFileText));
                }

                Debug.WriteLine("Reading Controllers Supported Json completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json: " + ex.Message);
            }
        }

        //Read from Json file (Deserialize)
        void JsonLoadList_ControllerProfile()
        {
            try
            {
                //Remove all the current controllers
                vDirectControllersProfile.Clear();

                string JsonFile = File.ReadAllText(@"Profiles\User\DirectControllersProfile.json");
                ControllerProfile[] JsonList = JsonConvert.DeserializeObject<ControllerProfile[]>(JsonFile);
                foreach (ControllerProfile Controller in JsonList)
                {
                    try
                    {
                        vDirectControllersProfile.Add(Controller);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Controllers Profile Json completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json: " + ex.Message);
            }
        }
    }
}