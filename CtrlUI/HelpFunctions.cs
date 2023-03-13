using ArnoldVinkCode;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check if DirectXInput is running and the keyboard setting is enabled
        bool CheckKeyboardEnabled()
        {
            try
            {
                //Load the current DirectXInput settings
                vConfigurationDirectXInput = SettingLoadConfig("DirectXInput.exe.csettings");

                bool processDirectXInputRunning = Check_RunningProcessByName("DirectXInput", true);
                bool ShortcutKeyboardPopup = SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool));
                if (ShortcutKeyboardPopup && processDirectXInputRunning)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Update controller help
        void UpdateControllerHelp()
        {
            try
            {
                //Check if DirectXInput is running
                bool processDirectXInputRunning = Check_RunningProcessByName("DirectXInput", true);

                AVActions.DispatcherInvoke(delegate
                {
                    //Check if there is any controller connected
                    if (!vControllerAnyConnected())
                    {
                        grid_ControllerHelp.Visibility = Visibility.Collapsed;
                        image_MainMenu_Close.Visibility = Visibility.Collapsed;
                        image_Welcome_Close.Visibility = Visibility.Collapsed;
                        image_Manage_Close.Visibility = Visibility.Collapsed;
                        image_Manage_Save.Visibility = Visibility.Collapsed;
                        image_Settings_Close.Visibility = Visibility.Collapsed;
                        image_Monitor_Close.Visibility = Visibility.Collapsed;
                        image_Help_Close.Visibility = Visibility.Collapsed;
                        image_ColorPicker_Close.Visibility = Visibility.Collapsed;
                        image_FilePicker_Close.Visibility = Visibility.Collapsed;
                        image_FilePicker_Up.Visibility = Visibility.Collapsed;
                        image_FilePicker_Left.Visibility = Visibility.Collapsed;
                        image_FilePicker_Back.Visibility = Visibility.Collapsed;
                        image_FilePicker_Start.Visibility = Visibility.Collapsed;
                        image_ProfileManager_Close.Visibility = Visibility.Collapsed;
                        image_MoveApplication_Close.Visibility = Visibility.Collapsed;
                        image_MessageBox_Close.Visibility = Visibility.Collapsed;
                        image_TextInput_Close.Visibility = Visibility.Collapsed;
                        image_TextInput_Reset.Visibility = Visibility.Collapsed;
                        image_TextInput_Set.Visibility = Visibility.Collapsed;
                        image_HowLongToBeat_Close.Visibility = Visibility.Collapsed;
                        image_Category_LB.Visibility = Visibility.Collapsed;
                        image_Category_RB.Visibility = Visibility.Collapsed;
                        image_Settings_LB.Visibility = Visibility.Collapsed;
                        image_Settings_RB.Visibility = Visibility.Collapsed;
                        image_Search_Reset.Visibility = Visibility.Collapsed;
                        gif_FilePicker_Loading.Height = 30;
                        gif_FilePicker_Loading.Width = 30;
                        return;
                    }
                    else
                    {
                        image_MainMenu_Close.Visibility = Visibility.Visible;
                        image_Welcome_Close.Visibility = Visibility.Visible;
                        image_Manage_Close.Visibility = Visibility.Visible;
                        image_Manage_Save.Visibility = Visibility.Visible;
                        image_Settings_Close.Visibility = Visibility.Visible;
                        image_Monitor_Close.Visibility = Visibility.Visible;
                        image_Help_Close.Visibility = Visibility.Visible;
                        image_ColorPicker_Close.Visibility = Visibility.Visible;
                        image_FilePicker_Close.Visibility = Visibility.Visible;
                        image_FilePicker_Up.Visibility = Visibility.Visible;
                        image_FilePicker_Left.Visibility = Visibility.Visible;
                        image_FilePicker_Back.Visibility = Visibility.Visible;
                        image_FilePicker_Start.Visibility = Visibility.Visible;
                        image_ProfileManager_Close.Visibility = Visibility.Visible;
                        image_MoveApplication_Close.Visibility = Visibility.Visible;
                        image_MessageBox_Close.Visibility = Visibility.Visible;
                        image_TextInput_Close.Visibility = Visibility.Visible;
                        image_TextInput_Reset.Visibility = Visibility.Visible;
                        image_TextInput_Set.Visibility = Visibility.Visible;
                        image_HowLongToBeat_Close.Visibility = Visibility.Visible;
                        image_Category_LB.Visibility = Visibility.Visible;
                        image_Category_RB.Visibility = Visibility.Visible;
                        image_Settings_LB.Visibility = Visibility.Visible;
                        image_Settings_RB.Visibility = Visibility.Visible;
                        image_Search_Reset.Visibility = Visibility.Visible;
                        gif_FilePicker_Loading.Height = 75;
                        gif_FilePicker_Loading.Width = 75;
                    }

                    //Check if the help setting is enabled or disabled
                    if (SettingLoad(vConfigurationCtrlUI, "HideControllerHelp", typeof(bool)))
                    {
                        grid_ControllerHelp.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        grid_ControllerHelp.Visibility = Visibility.Visible;
                    }

                    //Check DirectXInput settings
                    bool ShortcutKeyboardPopup = SettingLoad(vConfigurationDirectXInput, "ShortcutKeyboardPopup", typeof(bool));
                    if (processDirectXInputRunning && ShortcutKeyboardPopup)
                    {
                        sp_ControllerHelpGuideHold.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        sp_ControllerHelpGuideHold.Visibility = Visibility.Collapsed;
                    }

                    bool ShortcutScreenshotController = SettingLoad(vConfigurationDirectXInput, "ShortcutScreenshotController", typeof(bool));
                    if (processDirectXInputRunning && ShortcutScreenshotController)
                    {
                        sp_ControllerHelpScreenshot.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        sp_ControllerHelpScreenshot.Visibility = Visibility.Collapsed;
                    }

                    bool ShortcutAltEnter = SettingLoad(vConfigurationDirectXInput, "ShortcutAltEnter", typeof(bool));
                    if (processDirectXInputRunning && ShortcutAltEnter)
                    {
                        sp_ControllerHelpAltEnter.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        sp_ControllerHelpAltEnter.Visibility = Visibility.Collapsed;
                    }

                    bool ShortcutAltTab = SettingLoad(vConfigurationDirectXInput, "ShortcutAltTab", typeof(bool));
                    if (processDirectXInputRunning && ShortcutAltTab)
                    {
                        sp_ControllerHelpAltTab.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        sp_ControllerHelpAltTab.Visibility = Visibility.Collapsed;
                    }

                    //Update controller help text
                    UpdateControllerHelpText("Quick launch", "Sort", string.Empty, "Interact", "Hide", "Search", "Menu", string.Empty, string.Empty);
                });
            }
            catch { }
        }

        //Update controller help text
        void UpdateControllerHelpText(string Up, string Right, string Down, string Left, string Guide, string Back, string Start, string Lb, string Rb)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Up))
                {
                    txt_ControllerHelpUp.Text = Up;
                    sp_ControllerHelpUp.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpUp.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Right))
                {
                    txt_ControllerHelpRight.Text = Right;
                    sp_ControllerHelpRight.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpRight.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Down))
                {
                    txt_ControllerHelpDown.Text = Down;
                    sp_ControllerHelpDown.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpDown.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Left))
                {
                    txt_ControllerHelpLeft.Text = Left;
                    sp_ControllerHelpLeft.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpLeft.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Guide))
                {
                    txt_ControllerHelpGuide.Text = Guide;
                    sp_ControllerHelpGuide.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpGuide.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Back))
                {
                    txt_ControllerHelpBack.Text = Back;
                    sp_ControllerHelpBack.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpBack.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Start))
                {
                    txt_ControllerHelpStart.Text = Start;
                    sp_ControllerHelpStart.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpStart.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Lb))
                {
                    txt_ControllerHelpLb.Text = Lb;
                    sp_ControllerHelpLb.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpLb.Visibility = Visibility.Collapsed;
                }

                if (!string.IsNullOrWhiteSpace(Rb))
                {
                    txt_ControllerHelpRb.Text = Rb;
                    sp_ControllerHelpRb.Visibility = Visibility.Visible;
                }
                else
                {
                    sp_ControllerHelpRb.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        //Load - Help text
        void LoadHelp()
        {
            try
            {
                if (sp_Help.Children.Count == 0)
                {
                    sp_Help.Children.Add(new TextBlock() { Text = "My controller is not working properly?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "Make sure that your controller is connected through DirectXInput otherwise this application will not recognize your controller, if you have multiple controllers connected press on the 'Guide' button to activate it.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nHow can I use my controller as XInput?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "You can use the included DirectXInput tool that will convert your direct input controller in to a Xbox controller so it can be used with almost every application available.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nHow can I add information to my roms?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "You can add images and descriptions to your roms directory by naming a png image or txt file the same as the beginning of a rom file, for example image 'Super Mario 64.png' will be shown for 'Super Mario 64 (U) [!].z64', you can also store rom images and descriptions in the 'Assets\\User\\Emulators' folder to keep the information portable.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nHelp my emulator does not load the rom?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "Some emulators might need an extra launch argument to load the rom, to do this add the required launch argument to the application, for example some emulators need to start with the argument '-rom' so the emulator knows a rom will be loaded.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nHow can I enter text into a textbox with my controller?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "When a controller is connected and a textbox is selected you can press on the Cross or X button to open the text input popup which will also launch the on screen keyboard automatically for easy text input.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nWhich buttons can I use on my controller?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "LB or RB: Moves between interface buttons.\r\nTriggers: Changes your system sound volume step by step.\r\nStick click: Go to the first or last application in the list.\r\nBack: Opens and closes the main menu for you.\r\nStart: Opens and closes the search menu or sets the controller as the active controller.\r\nGuide + Start: Disconnects the controller when DirectXInput is running.\r\nGuide hold: Opens the on screen keyboard for windowed applications.\r\nGuide press: Switches between CtrlUI and running application.\r\nSquare or X: Interact with selected application from the list.\r\nTriangle or Y: Shows the quick launch prompt.\r\nRound or B: Refreshes the processes and shortcuts list.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nWhich buttons can I use on my keyboard?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "Windows + Caps Lock: Switches between CtrlUI and applications.\r\nF2: Shows the quick launch prompt.\r\nF3: Opens and closes the search menu.\r\nF4: Sort applications by name or number and date.\r\nF5: Refreshes the processes and shortcuts list.\r\nF6: Opens and closes the main menu for you.\r\nDelete: Interact with selected application from the list.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nOn screen keyboard does not function properly?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "The on screen keyboard only works with applications that aren't exclusive fullscreen, setting your application to launch in borderless fullscreen will solve this issue, some games that use anti cheat software may not be able to receive key clicks from the on screen keyboard.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nWhy does this application use a few percent of my cpu?", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "When the application is visible and activated the animations will causes the cpu to be used by a few percent, when the application is not activated it will drop down to almost no cpu usage.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nSupport and bug reporting", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "When you are walking into any problems or bugs you can go to the support page here: https://support.arnoldvink.com so I can try to help you out and get everything working.", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nDeveloper donation support", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "If you appreciate my project and want to support me with my projects you can make a donation through https://donation.arnoldvink.com", Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });

                    //Set the version text
                    sp_Help.Children.Add(new TextBlock() { Text = "\r\nApplication made by Arnold Vink", Style = (Style)Application.Current.Resources["TextBlockWhite"], FontSize = (double)Application.Current.Resources["TextSizeLarge"] });
                    sp_Help.Children.Add(new TextBlock() { Text = "Version: v" + Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0], Style = (Style)Application.Current.Resources["TextBlockGray"], FontSize = (double)Application.Current.Resources["TextSizeMedium"], TextWrapping = TextWrapping.Wrap });
                }
            }
            catch { }
        }

        void Button_Help_ProjectWebsite_Click(object sender, RoutedEventArgs e) { Process.Start("https://projects.arnoldvink.com"); }
        void Button_Help_OpenDonation_Click(object sender, RoutedEventArgs e) { Process.Start("https://donation.arnoldvink.com"); }
    }
}