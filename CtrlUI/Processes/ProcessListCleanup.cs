﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Cleanup no longer running combined processes
        void ProcessListCleanupCombined(List<ProcessMulti> processMultiList, IEnumerable<DataBindApp> combinedAppLists)
        {
            try
            {
                foreach (DataBindApp dataBindApp in combinedAppLists)
                {
                    try
                    {
                        //Remove closed processes
                        dataBindApp.ProcessMulti.RemoveAll(x => !processMultiList.Any(y => y.Identifier == x.Identifier));

                        //Check the running count
                        int processCount = dataBindApp.ProcessMulti.Count();

                        //Remove invalid processes
                        if (processCount > 1)
                        {
                            dataBindApp.ProcessMulti.RemoveAll(x => x.WindowHandleMain == IntPtr.Zero);
                            processCount = dataBindApp.ProcessMulti.Count();
                        }

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
        async Task ProcessListCleanupList(List<IntPtr> activeProcessesWindow)
        {
            try
            {
                Func<DataBindApp, bool> filterProcessApp = x => x.Category == AppCategory.Process && (!x.ProcessMulti.Any() || x.ProcessMulti.Any(z => !activeProcessesWindow.Contains(z.WindowHandleMain)) || x.ProcessMulti.Any(z => z.WindowHandleMain == IntPtr.Zero));
                await ListBoxRemoveAll(lb_Processes, List_Processes, filterProcessApp);
                await ListBoxRemoveAll(lb_Search, List_Search, filterProcessApp);
            }
            catch { }
        }
    }
}