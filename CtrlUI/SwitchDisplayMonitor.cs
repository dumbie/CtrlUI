using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVSwitchDisplayMonitor;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

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
                    Answer0.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1);
                    Answer0.Name = displayMonitor.Name;
                    Answers.Add(Answer0);
                }

                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1);
                Answer1.Name = "Primary monitor";
                Answers.Add(Answer1);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1);
                Answer2.Name = "Duplicate mode";
                Answers.Add(Answer2);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1);
                Answer3.Name = "Extend mode";
                Answers.Add(Answer3);

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                //Show the messagebox prompt
                DataBindString ResultMultiple = await Popup_Show_MessageBox("Switch display monitor", "", "Please select the display monitor you want to use.", Answers);
                if (ResultMultiple != null)
                {
                    if (ResultMultiple == Answer1)
                    {
                        Popup_Show_Status("MonitorSwitch", "Switching primary monitor");
                        EnableMonitorFirst();
                    }
                    else if (ResultMultiple == Answer2)
                    {
                        Popup_Show_Status("MonitorSwitch", "Cloning display monitor");
                        EnableMonitorCloneMode();
                    }
                    else if (ResultMultiple == Answer3)
                    {
                        Popup_Show_Status("MonitorSwitch", "Extending display monitor");
                        EnableMonitorExtendMode();
                    }
                    else if (ResultMultiple == cancelString)
                    {
                        return;
                    }
                    else
                    {
                        DisplayMonitorSummary changeDevice = monitorsList.Where(x => x.Name.ToLower() == ResultMultiple.Name.ToLower()).FirstOrDefault();
                        if (changeDevice != null)
                        {
                            Popup_Show_Status("MonitorSwitch", "Switching display monitor");
                            if (!SwitchPrimaryMonitor(changeDevice.Id))
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