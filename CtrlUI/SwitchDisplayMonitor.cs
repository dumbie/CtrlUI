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
                    DataBindString AnswerMonitor = new DataBindString();
                    AnswerMonitor.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                    AnswerMonitor.Name = displayMonitor.Name;
                    Answers.Add(AnswerMonitor);
                }

                DataBindString AnswerPrimary = new DataBindString();
                AnswerPrimary.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                AnswerPrimary.Name = "Primary monitor";
                Answers.Add(AnswerPrimary);

                DataBindString AnswerSecondary = new DataBindString();
                AnswerSecondary.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                AnswerSecondary.Name = "Secondary monitor";
                Answers.Add(AnswerSecondary);

                DataBindString AnswerDuplicate = new DataBindString();
                AnswerDuplicate.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                AnswerDuplicate.Name = "Duplicate mode";
                Answers.Add(AnswerDuplicate);

                DataBindString AnswerExtend = new DataBindString();
                AnswerExtend.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/MonitorSwitch.png" }, IntPtr.Zero, -1, 0);
                AnswerExtend.Name = "Extend mode";
                Answers.Add(AnswerExtend);

                //Show the messagebox prompt
                DataBindString messageResult = await Popup_Show_MessageBox("Switch display monitor", "", "Please select the display monitor you want to use.", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerPrimary)
                    {
                        Popup_Show_Status("MonitorSwitch", "Switching primary monitor");
                        EnableMonitorFirst();
                    }
                    else if (messageResult == AnswerSecondary)
                    {
                        Popup_Show_Status("MonitorSwitch", "Switching secondary monitor");
                        EnableMonitorSecond();
                    }
                    else if (messageResult == AnswerDuplicate)
                    {
                        Popup_Show_Status("MonitorSwitch", "Cloning display monitor");
                        EnableMonitorCloneMode();
                    }
                    else if (messageResult == AnswerExtend)
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