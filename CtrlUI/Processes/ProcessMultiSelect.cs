using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessClasses;
using static ArnoldVinkCode.ProcessFunctions;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Select a process multi from the list
        async Task<ProcessMulti> SelectProcessMulti(DataBindApp dataBindApp, bool selectProcess)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                if (dataBindApp.ProcessMulti.Any())
                {
                    if (selectProcess && dataBindApp.ProcessMulti.Count > 1)
                    {
                        foreach (ProcessMulti multiProcess in dataBindApp.ProcessMulti)
                        {
                            try
                            {
                                //Get the process title
                                string ProcessTitle = GetWindowTitleFromWindowHandle(multiProcess.WindowHandle);
                                if (ProcessTitle == "Unknown") { ProcessTitle += " (Hidden)"; }
                                if (multiAnswers.Where(x => x.Name.ToLower() == ProcessTitle.ToLower()).Any()) { ProcessTitle += " (" + multiAnswers.Count + ")"; }

                                DataBindString AnswerApp = new DataBindString();
                                AnswerApp.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                                AnswerApp.Name = ProcessTitle;
                                multiAnswers.Add(AnswerApp);
                            }
                            catch { }
                        }

                        DataBindString Answer1 = new DataBindString();
                        Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/App.png" }, IntPtr.Zero, -1);
                        Answer1.Name = "Launch new instance";
                        multiAnswers.Add(Answer1);

                        DataBindString Answer2 = new DataBindString();
                        Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1);
                        Answer2.Name = "Close all the instances";
                        multiAnswers.Add(Answer2);

                        DataBindString cancelString = new DataBindString();
                        cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                        cancelString.Name = "Cancel";
                        multiAnswers.Add(cancelString);

                        DataBindString Result = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple running instances", "", "Please select the instance that you wish to interact with:", multiAnswers);
                        if (Result != null)
                        {
                            if (Result == Answer2)
                            {
                                //Get the first multi process type
                                ProcessType processType = dataBindApp.ProcessMulti.FirstOrDefault().Type;

                                //Return close all with process type
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Action = "CloseAll";
                                processMultiNew.Type = processType;
                                return processMultiNew;
                            }
                            else if (Result == cancelString)
                            {
                                ProcessMulti processMultiNew = new ProcessMulti();
                                processMultiNew.Action = "Cancel";
                                return processMultiNew;
                            }
                            else
                            {
                                return dataBindApp.ProcessMulti[multiAnswers.IndexOf(Result)];
                            }
                        }
                    }
                    else
                    {
                        return dataBindApp.ProcessMulti.FirstOrDefault();
                    }
                }
            }
            catch { }
            return null;
        }
    }
}