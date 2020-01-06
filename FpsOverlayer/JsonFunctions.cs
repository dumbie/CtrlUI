using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;

namespace FpsOverlayer
{
    partial class JsonFunctions
    {
        //Read blacklist process from Json file (Deserialize)
        public static void JsonLoadFpsBlacklistProcess()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\FpsBlacklistProcess.json");
                vFpsBlacklistProcess = JsonConvert.DeserializeObject<string[]>(JsonFile);

                Debug.WriteLine("Reading Json blacklist process completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json blacklist process: " + ex.Message);
            }
        }

        //Read position process from Json file (Deserialize)
        public static void JsonLoadFpsPositionProcess()
        {
            try
            {
                string JsonFile = File.ReadAllText(@"Profiles\FpsPositionProcess.json");
                vFpsPositionProcess = JsonConvert.DeserializeObject<ObservableCollection<FpsPositionProcess>>(JsonFile);

                Debug.WriteLine("Reading Json position process completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed Reading Json position process: " + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        public static void JsonSaveObject(object serializeObject, string profileName)
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