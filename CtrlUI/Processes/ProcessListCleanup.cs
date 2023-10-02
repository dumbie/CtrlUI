using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Cleanup no longer running list apps
        void ProcessListCleanupApps(IEnumerable<int> processIdentifiers, IEnumerable<DataBindApp> combinedAppLists)
        {
            try
            {
                foreach (DataBindApp dataBindApp in combinedAppLists)
                {
                    try
                    {
                        //Remove closed processes
                        Predicate<ProcessMulti> filterProcessApp = x => !processIdentifiers.Any(y => y == x.Identifier);
                        dataBindApp.ProcessMulti.RemoveAll(filterProcessApp);

                        //Remove invalid window processes
                        if (dataBindApp.ProcessMulti.Any(x => x.WindowHandleMain != IntPtr.Zero))
                        {
                            dataBindApp.ProcessMulti.RemoveAll(x => x.WindowHandleMain == IntPtr.Zero);
                        }

                        //Check process running count
                        int processCount = dataBindApp.ProcessMulti.Count();

                        //Update the running count text
                        if (processCount > 1)
                        {
                            dataBindApp.StatusProcessCount = Convert.ToString(processCount);
                        }
                        else
                        {
                            dataBindApp.StatusProcessCount = string.Empty;
                        }

                        //Update the running status
                        if (processCount == 0)
                        {
                            dataBindApp.ResetStatus(false);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        //Cleanup no longer running list processes
        async Task ProcessListCleanupProcesses(IEnumerable<int> processIdentifiers, IEnumerable<IntPtr> processWindowHandles)
        {
            try
            {
                foreach (DataBindApp dataBindApp in List_Processes)
                {
                    try
                    {
                        //Remove closed processes
                        Predicate<ProcessMulti> filterProcessApp = x => !processIdentifiers.Any(y => y == x.Identifier) || !processWindowHandles.Any(y => y == x.WindowHandleMain);
                        dataBindApp.ProcessMulti.RemoveAll(filterProcessApp);

                        //Check process running count
                        if (!dataBindApp.ProcessMulti.Any())
                        {
                            await ListBoxRemoveItem(lb_Processes, List_Processes, dataBindApp, true);
                            await ListBoxRemoveItem(lb_Search, List_Search, dataBindApp, true);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}