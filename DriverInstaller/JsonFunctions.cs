using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace DriverInstaller
{
    public partial class WindowMain
    {
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
    }
}