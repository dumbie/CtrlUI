using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.FocusFunctions;
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
                    PlayInterfaceSound(vConfigurationCtrlUI, "PromptOpen", false);

                    //Save the previous focus element
                    FrameworkElementFocusSave(vMessageBoxElementFocus, null);
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
                    grid_MessageBox_Border.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Question.Text = string.Empty;
                    grid_MessageBox_Question.Visibility = Visibility.Collapsed;
                    grid_MessageBox_Border.Visibility = Visibility.Collapsed;
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
                await ListboxFocusIndex(lb_MessageBox, true, false, -1, vProcessCurrent.MainWindowHandle);

                //Wait for user messagebox input
                while (vMessageBoxResult == null && !vMessageBoxCancelled) { await Task.Delay(500); }
                if (vMessageBoxCancelled) { return null; }

                //Close and reset the popup
                await Popup_Close_MessageBox();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MessageBox show error: " + ex.Message);
            }
            return vMessageBoxResult;
        }

        //Close and reset messageboxpopup
        async Task Popup_Close_MessageBox()
        {
            try
            {
                //Play the closing sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PromptClose", false);

                //Reset the popup variables
                vMessageBoxCancelled = true;
                //vMessageBoxResult = null;
                vMessageBoxOpen = false;

                //Hide the popup
                Popup_Hide_Element(grid_Popup_MessageBox);

                //Focus on the previous focus element
                await FrameworkElementFocusFocus(vMessageBoxElementFocus, vProcessCurrent.MainWindowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MessageBox close error: " + ex.Message);
            }
        }
    }
}