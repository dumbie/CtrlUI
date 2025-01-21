using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
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
                //Filter game name
                string filterSearchGame = FilterNameGame(searchGame, true, false, true, 0);

                //Show the text input popup
                filterSearchGame = await Popup_ShowHide_TextInput("How long to beat search", filterSearchGame, "Search how long to beat for the game", true);
                if (string.IsNullOrWhiteSpace(filterSearchGame))
                {
                    Debug.WriteLine("No search term entered.");
                    return;
                }

                //Check if the popup is already open
                if (!vHowLongToBeatOpen)
                {
                    //Play the opening sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PromptOpen", false, false);

                    //Save the previous focus element
                    AVFocusDetailsSave(vHowLongToBeatElementFocus, null);
                }

                //Clear current search results
                lb_HowLongToBeat.Items.Clear();

                //Show loading progress
                textblock_HowLongToBeat_Unknown.Text = "Downloading gameplay time for " + searchGame;
                gif_HowLongToBeat_Loading.Show();
                grid_HowLongToBeat.Visibility = Visibility.Collapsed;
                stackpanel_HowLongToBeat_Status.Visibility = Visibility.Visible;

                //Reset HowLongToBeat variables
                vHowLongToBeatOpen = true;

                //Show the popup
                Popup_Show_Element(grid_Popup_HowLongToBeat);

                //Search for game
                ApiHltbSearchResultHtml searchResult = await ApiHowLongToBeat_Search_Html(filterSearchGame);

                //Read games from result
                if (searchResult != null && searchResult.data != null)
                {
                    foreach (ApiHltbSearchResultHtml.Data hltbData in searchResult.data)
                    {
                        try
                        {
                            if (hltbData.comp_all_count <= 0)
                            {
                                Debug.WriteLine("Unknown gameplay time: " + hltbData.game_name);
                            }
                            else
                            {
                                DataBindString dataBindString = new DataBindString();

                                //Set name and year
                                dataBindString.Data1 = hltbData.game_name;
                                if (hltbData.release_world > 0)
                                {
                                    dataBindString.Data1 += " (" + hltbData.release_world + ")";
                                }

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

                                //Check completed times
                                if (hltbData.comp_all_count > 0)
                                {
                                    dataBindString.Data5 = hltbData.comp_all_count + "x";
                                }
                                else
                                {
                                    dataBindString.Data5 = "None";
                                }

                                //Add result to the list
                                lb_HowLongToBeat.Items.Add(dataBindString);
                            }
                        }
                        catch { }
                    }
                }

                //Check if there are any results
                gif_HowLongToBeat_Loading.Hide();
                if (lb_HowLongToBeat.Items.Count > 0)
                {
                    grid_HowLongToBeat.Visibility = Visibility.Visible;
                    stackpanel_HowLongToBeat_Status.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textblock_HowLongToBeat_Unknown.Text = "Unknown gameplay time for " + searchGame;
                    grid_HowLongToBeat.Visibility = Visibility.Collapsed;
                    stackpanel_HowLongToBeat_Status.Visibility = Visibility.Visible;
                }

                //Focus on first listbox answer
                await ListBoxFocusIndex(lb_HowLongToBeat, false, 0, vProcessCurrent.WindowHandleMain);
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
                await AVFocusDetailsFocus(vHowLongToBeatElementFocus, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HowLongToBeat close error: " + ex.Message);
            }
        }
    }
}