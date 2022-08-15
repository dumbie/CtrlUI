using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LibraryShared
{
    public class JsonFunctions
    {
        //Read Json from path (Deserialize)
        public static void JsonLoadPath<T>(ref T deserializeTarget, string filePath)
        {
            try
            {
                string jsonFile = File.ReadAllText(filePath);
                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFile);
                Debug.WriteLine("Completed reading json file: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json file: " + filePath + "/" + ex.Message);
            }
        }

        //Read Json from profile (Deserialize)
        public static void JsonLoadSingle<T>(ref T deserializeTarget, string profileName)
        {
            try
            {
                string jsonFile = File.ReadAllText(@"Profiles\" + profileName + ".json");
                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFile);
                Debug.WriteLine("Completed reading json file: " + profileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json file: " + profileName + "/" + ex.Message);
            }
        }

        //Read Json from profile (Deserialize)
        public static void JsonLoadMulti<T>(ICollection<T> targetList, string loadDirectory)
        {
            try
            {
                //Clear loaded json
                targetList.Clear();

                //Add all the supported controllers
                string[] jsonFiles = Directory.GetFiles(@"Profiles\" + loadDirectory, "*.json");
                foreach (string jsonFile in jsonFiles)
                {
                    string jsonFileText = File.ReadAllText(jsonFile);
                    targetList.Add(JsonConvert.DeserializeObject<T>(jsonFileText));
                }
                Debug.WriteLine("Completed reading json files from: " + loadDirectory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading json files: " + loadDirectory + "/" + ex.Message);
            }
        }

        //Read Json from embedded file (Deserialize)
        public static void JsonLoadEmbedded<T>(Assembly assembly, ref T deserializeTarget, string resourcePath)
        {
            try
            {
                string jsonFile = string.Empty;
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

                //Check directory
                string filePath = @"Profiles\" + profileName + ".json";
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                    Debug.WriteLine("Created Json directory: " + directoryName);
                }

                //Save to file
                File.WriteAllText(filePath, serializedObject);
                Debug.WriteLine("Saving Json " + profileName + " completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json " + profileName + ": " + ex.Message);
            }
        }

        //Remove Json file
        public static void JsonRemoveFile(string profileName)
        {
            try
            {
                string filePath = @"Profiles\" + profileName + ".json";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                Debug.WriteLine("Removed Json file: " + profileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing json file: " + ex.Message);
            }
        }
    }
}