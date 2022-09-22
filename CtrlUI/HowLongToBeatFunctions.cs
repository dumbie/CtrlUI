using ArnoldVinkCode;
using System;
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
        //Show and close HowLongToBeat Popup
        public async Task Popup_Show_HowLongToBeat(string searchGame)
        {
            try
            {
                //Check if the popup is already open
                if (!vHowLongToBeatOpen)
                {
                    //Play the opening sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PromptOpen", false, false);

                    //Save the previous focus element
                    FrameworkElementFocusSave(vHowLongToBeatElementFocus, null);
                }

                //Reset HowLongToBeat variables
                vHowLongToBeatOpen = true;

                //Clear current search results
                lb_HowLongToBeat.Items.Clear();

                //Fix add loading progress
                //Fix filter game name

                //Search for game
                ApiHltbSearchResult searchResult = await ApiHowLongToBeat_Search(searchGame);

                //Read games from result
                if (searchResult != null && searchResult.data != null)
                {
                    foreach (ApiHltbSearchResult.Data hltbData in searchResult.data)
                    {
                        try
                        {
                            if (hltbData.comp_all_count <= 0)
                            {
                                Debug.WriteLine("Unknown gameplay time: " + searchGame);
                            }
                            else
                            {
                                DataBindString dataBindString = new DataBindString();

                                //Set name and completed times
                                dataBindString.Data1 = hltbData.game_name + " (" + hltbData.comp_all_count + "x)";

                                //Check main story time
                                if (hltbData.comp_main > 0)
                                {
                                    dataBindString.Data2 = AVFunctions.SecondsToHms(hltbData.comp_main, true, false);
                                }
                                else
                                {
                                    dataBindString.Data2 = "Unknown";
                                }

                                //Check main + extra time
                                if (hltbData.comp_plus > 0)
                                {
                                    dataBindString.Data3 = AVFunctions.SecondsToHms(hltbData.comp_plus, true, false);
                                }
                                else
                                {
                                    dataBindString.Data3 = "Unknown";
                                }

                                //Check completionist time
                                if (hltbData.comp_100 > 0)
                                {
                                    dataBindString.Data4 = AVFunctions.SecondsToHms(hltbData.comp_100, true, false);
                                }
                                else
                                {
                                    dataBindString.Data4 = "Unknown";
                                }

                                //Add result to the list
                                lb_HowLongToBeat.Items.Add(dataBindString);
                            }
                        }
                        catch { }
                    }
                }

                //Check if there are any results
                if (lb_HowLongToBeat.Items.Count > 0)
                {
                    grid_HowLongToBeat.Visibility = Visibility.Visible;
                    textblock_HowLongToBeat_Unknown.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid_HowLongToBeat.Visibility = Visibility.Collapsed;
                    textblock_HowLongToBeat_Unknown.Text = "Unknown gameplay time: " + searchGame;
                    textblock_HowLongToBeat_Unknown.Visibility = Visibility.Visible;
                }

                //Show the popup
                Popup_Show_Element(grid_Popup_HowLongToBeat);

                //Focus on first listbox answer
                await ListboxFocusIndex(lb_HowLongToBeat, true, false, -1, vProcessCurrent.MainWindowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HowLongToBeat show error: " + ex.Message);
            }
        }

        //Close and reset HowLongToBeatpopup
        async Task Popup_Close_HowLongToBeat()
        {
            try
            {
                //Play the closing sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PromptClose", false, false);

                //Reset the popup variables
                vHowLongToBeatOpen = false;

                //Hide the popup
                Popup_Hide_Element(grid_Popup_HowLongToBeat);

                //Focus on the previous focus element
                await FrameworkElementFocusFocus(vHowLongToBeatElementFocus, vProcessCurrent.MainWindowHandle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HowLongToBeat close error: " + ex.Message);
            }
        }
    }
}