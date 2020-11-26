using ArnoldVinkCode;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Generate summary string from ApiIGDBPlatformVersions
        string ApiIGDB_ConsoleSummaryString(ApiIGDBPlatformVersions infoConsole)
        {
            string summaryString = string.Empty;
            try
            {
                //Cpu
                if (!string.IsNullOrWhiteSpace(infoConsole.cpu))
                {
                    summaryString += "\nCpu: " + infoConsole.cpu;
                }

                //Memory
                if (!string.IsNullOrWhiteSpace(infoConsole.memory))
                {
                    summaryString += "\nMem: " + infoConsole.memory;
                }

                //Graphics
                if (!string.IsNullOrWhiteSpace(infoConsole.graphics))
                {
                    summaryString += "\nGpu: " + infoConsole.graphics;
                }

                //Summary
                if (string.IsNullOrWhiteSpace(infoConsole.summary))
                {
                    summaryString += "\n\nThere is no description available.";
                }
                else
                {
                    summaryString += "\n\n" + infoConsole.summary;
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