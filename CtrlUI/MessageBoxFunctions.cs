using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ProcessWin32Functions;
using static CtrlUI.AppVariables;
using static CtrlUI.ImageFunctions;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show and close Messagebox Popup
        public async Task<DataBindString> Popup_Show_MessageBox(string Question, string Subtitle, string Description, List<DataBindString> Answers)
        {
            try
            {
                //Check if the popup is already open
                if (!vMessageBoxOpen)
                {
                    //Play the opening sound
                    PlayInterfaceSound(vInterfaceSoundVolume, "PromptOpen", false);

                    //Save the previous focus element
                    Popup_PreviousElementFocus_Save(vMessageBoxElementFocus, null);
                }

                //Reset messagebox variables
                vMessageBoxCancelled = false;
                vMessageBoxResult = null;
                vMessageBoxOpen = true;

                //Set messagebox question
                if (!string.IsNullOrWhiteSpace(Question))
                {
                    grid_MessageBox_Question.Text = Question;
                    grid_MessageBox_Question.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Question.Text = string.Empty;
                    grid_MessageBox_Question.Visibility = Visibility.Collapsed;
                }

                //Set messagebox subtitle
                if (!string.IsNullOrWhiteSpace(Subtitle))
                {
                    grid_MessageBox_Subtitle.Text = Subtitle;
                    grid_MessageBox_Subtitle.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Subtitle.Text = string.Empty;
                    grid_MessageBox_Subtitle.Visibility = Visibility.Collapsed;
                }

                //Set messagebox description
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    grid_MessageBox_Description.Text = Description;
                    grid_MessageBox_Description.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Description.Text = string.Empty;
                    grid_MessageBox_Description.Visibility = Visibility.Collapsed;
                }

                //Set the messagebox answers
                lb_MessageBox.ItemsSource = Answers;

                //Show the popup
                Popup_Show_Element(grid_Popup_MessageBox);

                //Focus on first listbox answer
                await ListboxFocus(lb_MessageBox, true, false, -1);

                //Wait for user messagebox input
                while (vMessageBoxResult == null && !vMessageBoxCancelled) { await Task.Delay(500); }
                if (vMessageBoxCancelled) { return null; }

                //Close and reset the popup
                await Popup_Close_MessageBox();
            }
            catch { }
            return vMessageBoxResult;
        }

        //Close and reset messageboxpopup
        async Task Popup_Close_MessageBox()
        {
            try
            {
                //Play the closing sound
                PlayInterfaceSound(vInterfaceSoundVolume, "PromptClose", false);

                //Reset the popup variables
                vMessageBoxCancelled = true;
                //vMessageBoxResult = null;
                vMessageBoxOpen = false;

                //Hide the popup
                Popup_Hide_Element(grid_Popup_MessageBox);

                //Focus on the previous focus element
                await Popup_PreviousElementFocus_Focus(vMessageBoxElementFocus);
            }
            catch { }
        }

        //Close application messagebox
        async Task Popup_Show_MessageBox_Shutdown()
        {
            try
            {
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString Answer1 = new DataBindString();
                Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Closing.png" }, IntPtr.Zero, -1, 0);
                Answer1.Name = "Close CtrlUI";
                Answers.Add(Answer1);

                DataBindString Answer4 = new DataBindString();
                Answer4.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Restart.png" }, IntPtr.Zero, -1, 0);
                Answer4.Name = "Restart CtrlUI";
                Answers.Add(Answer4);

                DataBindString Answer3 = new DataBindString();
                Answer3.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Shutdown.png" }, IntPtr.Zero, -1, 0);
                Answer3.Name = "Shutdown my PC";
                Answers.Add(Answer3);

                DataBindString Answer2 = new DataBindString();
                Answer2.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Restart.png" }, IntPtr.Zero, -1, 0);
                Answer2.Name = "Restart my PC";
                Answers.Add(Answer2);

                DataBindString messageResult = await Popup_Show_MessageBox("Would you like to close CtrlUI or shutdown your PC?", "If you have DirectXInput running and a controller connected you can launch CtrlUI by pressing on the 'Guide' button.", "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == Answer1)
                    {
                        Popup_Show_Status("Closing", "Closing CtrlUI");
                        await Application_Exit(true);
                    }
                    else if (messageResult == Answer4)
                    {
                        Popup_Show_Status("Closing", "Restarting CtrlUI");
                        await Application_Restart();
                    }
                    else if (messageResult == Answer2)
                    {
                        Popup_Show_Status("Shutdown", "Restarting your PC");

                        //Close all other launchers
                        await CloseLaunchers(true);

                        //Restart the PC
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/r /t 0", false, true);

                        //Close CtrlUI
                        await Application_Exit(true);
                    }
                    else if (messageResult == Answer3)
                    {
                        Popup_Show_Status("Shutdown", "Shutting down your PC");

                        //Close all other launchers
                        await CloseLaunchers(true);

                        //Shutdown the PC
                        await ProcessLauncherWin32Async(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\shutdown.exe", "", "/s /t 0", false, true);

                        //Close CtrlUI
                        await Application_Exit(true);
                    }
                }
            }
            catch { }
        }
    }
}