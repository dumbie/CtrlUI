using ArnoldVinkCode;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate summary string from ApiIGDBPlatformVersions
        string ApiIGDB_PlatformSummaryString(ApiIGDBPlatformVersions infoPlatform)
        {
            string summaryString = string.Empty;
            try
            {
                //Cpu
                if (!string.IsNullOrWhiteSpace(infoPlatform.cpu))
                {
                    summaryString += "\nCpu: " + infoPlatform.cpu;
                }

                //Memory
                if (!string.IsNullOrWhiteSpace(infoPlatform.memory))
                {
                    summaryString += "\nMem: " + infoPlatform.memory;
                }

                //Graphics
                if (!string.IsNullOrWhiteSpace(infoPlatform.graphics))
                {
                    summaryString += "\nGpu: " + infoPlatform.graphics;
                }

                //Summary
                if (string.IsNullOrWhiteSpace(infoPlatform.summary))
                {
                    summaryString += "\n\nThere is no description available.";
                }
                else
                {
                    summaryString += "\n\n" + infoPlatform.summary;
                }

                //Remove first linebreak
                summaryString = AVFunctions.StringRemoveStart(summaryString, "\n");
            }
            catch { }
            if (string.IsNullOrWhiteSpace(summaryString)) { summaryString = "There is no description available."; }
            return summaryString;
        }
    }
}