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

        //Save position process to Json file (Serialize)
        public static void JsonSaveFpsPositionProcess()
        {
            try
            {
                string SerializedList = JsonConvert.SerializeObject(vFpsPositionProcess);
                File.WriteAllText(@"Profiles\FpsPositionProcess.json", SerializedList);

                Debug.WriteLine("Saving Json position process completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json position process: " + ex.Message);
            }
        }
    }
}