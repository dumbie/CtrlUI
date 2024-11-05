using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Select a process multi from the list
        async Task<ProcessMultiAction> SelectProcessMulti(DataBindApp dataBindApp)
        {
            try
            {
                List<DataBindString> multiAnswers = new List<DataBindString>();
                if (dataBindApp.ProcessMulti.Any())
                {
                    if (dataBindApp.ProcessMulti.Count > 1)
                    {
                        foreach (ProcessMulti multiProcess in dataBindApp.ProcessMulti)
                        {
                            try
                            {
                                //Get the process title
                                string processTitle = multiProcess.WindowTitleMain;
                                if (processTitle == "Unknown")
                                {
                                    processTitle += " (Hidden)";
                                }

                                DataBindString AnswerApp = new DataBindString();
                                AnswerApp.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/Process.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                                AnswerApp.Name = processTitle;
                                AnswerApp.NameSub = multiProcess.Identifier.ToString();
                                AnswerApp.Data1 = multiProcess;
                                multiAnswers.Add(AnswerApp);
                            }
                            catch { }
                        }

                        DataBindString AnswerLaunchNew = new DataBindString();
                        AnswerLaunchNew.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        AnswerLaunchNew.Name = "Launch new instance";
                        multiAnswers.Add(AnswerLaunchNew);

                        DataBindString AnswerCloseAll = new DataBindString();
                        AnswerCloseAll.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                        AnswerCloseAll.Name = "Close all the instances";
                        multiAnswers.Add(AnswerCloseAll);

                        DataBindString messageResult = await Popup_Show_MessageBox(dataBindApp.Name + " has multiple running instances", "", "Please select the instance that you wish to interact with:", multiAnswers);
                        if (messageResult != null)
                        {
                            if (messageResult == AnswerLaunchNew)
                            {
                                return new ProcessMultiAction() { Action = ProcessMultiActions.Launch };
                            }
                            else if (messageResult == AnswerCloseAll)
                            {
                                return new ProcessMultiAction() { Action = ProcessMultiActions.CloseAll };
                            }
                            else if (messageResult.Data1 != null)
                            {
                                return new ProcessMultiAction() { Action = ProcessMultiActions.Select, ProcessMulti = (ProcessMulti)messageResult.Data1 };
                            }
                        }

                        //Return cancel selection
                        return new ProcessMultiAction() { Action = ProcessMultiActions.Cancel };
                    }
                    else
                    {
                        Debug.WriteLine("Single process, returning process multi.");
                        return new ProcessMultiAction() { Action = ProcessMultiActions.Select, ProcessMulti = dataBindApp.ProcessMulti.FirstOrDefault() };
                    }
                }
                else
                {
                    Debug.WriteLine("No process, requesting process launch.");
                    return new ProcessMultiAction() { Action = ProcessMultiActions.Launch };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to select process multi: " + ex.Message);
                return new ProcessMultiAction() { Action = ProcessMultiActions.Cancel };
            }
        }
    }
}