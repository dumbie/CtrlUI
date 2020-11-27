using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LibraryShared
{
    public class JsonFunctions
    {
        //Read Json from profile (Deserialize)
        public static void JsonLoadProfile<T>(ref T deserializeTarget, string profileName)
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

        //Read Json from embedded file (Deserialize)
        public static void JsonLoadEmbedded<T>(ref T deserializeTarget, string resourcePath)
        {
            try
            {
                string jsonFile = string.Empty;
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonFile = reader.ReadToEnd();
                    }
                }

                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFile);
                Debug.WriteLine("Reading Json resource completed: " + resourcePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Reading Json resource failed: " + resourcePath + "/" + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        public static void JsonSaveObject(object serializeObject, string profileName)
        {
            try
            {
                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(serializeObject, jsonSettings);

                //Save to file
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