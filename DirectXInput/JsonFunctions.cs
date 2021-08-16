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

                Debug.WriteLine("Reading Controller Profile Json completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json: " + ex.Message);
            }
        }
    }
}