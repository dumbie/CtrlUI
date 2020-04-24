using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVDisplayMonitor;
using static LibraryShared.Classes;
using static LibraryShared.ImageFunctions;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task SwitchDisplayMonitor()
        {
            try
            {
                Debug.WriteLine("Listing all the connected display monitors.");

                List<DataBindString> Answers = new List<DataBindString>();

                //Get all the connected display monitors
                List<DisplayMonitorSummary> monitorsList = ListDisplayMonitors();

                //Add all display monitors to answers list
                foreach (DisplayMonitorSummary displayMonitor in monitorsList)
                {
                    DataBindString Answer0 = new DataBindString();
                    Answer0.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                    Answer0.Name = displayMonitor.Name;
                    Answers.Add(Answer0);
                }

                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Primary monitor";
                Answers.Add(Answer1);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                Answer2.Name = "Duplicate mode";
                Answers.Add(Answer2);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                Answer3.Name = "Extend mode";
                Answers.Add(Answer3);

                //Show the messagebox prompt
                DataBindString messageResult = await Popup_Show_MessageBox("Switch display monitor", "", "Please select the display monitor you want to use.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer1)
                    {
                        Popup_Show_Status("MonitorSwitch", "Switching primary monitor");
                        EnableMonitorFirst();
                    }
                    else if (messageResult == Answer2)
                    {
                        Popup_Show_Status("MonitorSwitch", "Cloning display monitor");
                        EnableMonitorCloneMode();
                    }
                    else if (messageResult == Answer3)
                    {
                        Popup_Show_Status("MonitorSwitch", "Extending display monitor");
                        EnableMonitorExtendMode();
                    }
                    else
                    {
                        DisplayMonitorSummary changeDevice = monitorsList.Where(x => x.Name.ToLower() == messageResult.Name.ToLower()).FirstOrDefault();
                        if (changeDevice != null)
                        {
                            Popup_Show_Status("MonitorSwitch", "Switching display monitor");
                            if (!SwitchPrimaryMonitor(changeDevice.Identifier))
                            {
                                Popup_Show_Status("MonitorSwitch", "Failed switching monitor");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the display monitors: " + ex.Message);
                Popup_Show_Status("MonitorSwitch", "No display monitors");
            }
        }
    }
}