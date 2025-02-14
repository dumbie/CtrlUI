using ArnoldVinkCode;
using System.Linq;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate summary string for ApiIGDBPlatforms
        string ApiIGDB_PlatformSummaryString(ApiIGDBPlatforms infoPlatforms)
        {
            string summaryString = string.Empty;
            try
            {
                //Get first versions
                ApiIGDBPlatformsVersions infoVersions = infoPlatforms.versions.FirstOrDefault();

                //Cpu
                if (!string.IsNullOrWhiteSpace(infoVersions.cpu))
                {
                    summaryString += "\nCpu: " + infoVersions.cpu;
                }

                //Memory
                if (!string.IsNullOrWhiteSpace(infoVersions.memory))
                {
                    summaryString += "\nMemory: " + infoVersions.memory;
                }

                //Graphics
                if (!string.IsNullOrWhiteSpace(infoVersions.graphics))
                {
                    summaryString += "\nGraphics: " + infoVersions.graphics;
                }

                //Output
                if (!string.IsNullOrWhiteSpace(infoVersions.output))
                {
                    summaryString += "\nOutput: " + infoVersions.output;
                }

                //Extras
                if (!string.IsNullOrWhiteSpace(infoVersions.media))
                {
                    summaryString += "\nExtras: " + infoVersions.media;
                }

                //Online
                if (!string.IsNullOrWhiteSpace(infoVersions.online))
                {
                    summaryString += "\nOnline: " + infoVersions.online;
                }

                //Operating System
                if (!string.IsNullOrWhiteSpace(infoVersions.os))
                {
                    summaryString += "\nOS: " + infoVersions.os;
                }

                //Summary
                if (!string.IsNullOrWhiteSpace(infoPlatforms.summary))
                {
                    summaryString += "\n\n" + infoPlatforms.summary;
                }
                else if (!string.IsNullOrWhiteSpace(infoVersions.summary))
                {
                    summaryString += "\n\n" + infoVersions.summary;
                }
                else
                {
                    summaryString += "\n\nThere is no description available.";
                }

                //Remove first line break
                summaryString = AVFunctions.StringRemoveStart(summaryString, "\n");
            }
            catch { }
            if (string.IsNullOrWhiteSpace(summaryString)) { summaryString = "There is no description available."; }
            return summaryString;
        }
    }
}