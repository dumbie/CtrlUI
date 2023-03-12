using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVActions;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static ArnoldVinkCode.AVSettings;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Process controller input
        public async Task ControllerInteraction(ControllerInput ControllerInput)
        {
            try
            {
                if (!vAppMinimized && vAppActivated)
                {
                    Controller_DPadPress(ControllerInput);
                    Controller_StickMovement(ControllerInput);
                    await Controller_ButtonPress(ControllerInput);
                    await Controller_TriggerPress(ControllerInput);
                }
            }
            catch { }
        }

        //Process active XInput controller buttons
        async Task<bool> Controller_ButtonPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Button)
                {
                    if (ControllerInput.ButtonA.PressedRaw)
                    {
                        Debug.WriteLine("Button: APressed");

                        await AVActions.DispatcherInvoke(async delegate
                        {
                            FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                            if (frameworkElement != null && frameworkElement.GetType() == typeof(TextBox))
                            {
                                //Launch the keyboard controller
                                if (vAppActivated && vControllerAnyConnected())
                                {
                                    await ShowHideKeyboardController(true);
                                }
                            }
                            else
                            {
                                //Press on the space bar
                                KeySendSingle(KeysVirtual.Space, vProcessCurrent.WindowHandleMain);
                            }
                        });

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        Debug.WriteLine("Button: BPressed");

                        if (Popup_Open_Any())
                        {
                            KeySendSingle(KeysVirtual.Escape, vProcessCurrent.WindowHandleMain);
                        }
                        else
                        {
                            await SortAppListsSwitch(false);
                        }

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        Debug.WriteLine("Button: YPressed");
                        ListCategory listAppCategory = (ListCategory)SettingLoad(vConfigurationCtrlUI, "ListAppCategory", typeof(int));

                        if (vTextInputOpen)
                        {
                            Debug.WriteLine("Resetting the text input popup.");
                            await AVActions.DispatcherInvoke(async delegate { await Popup_Reset_TextInput(true, string.Empty); });
                        }
                        else if (vFilePickerOpen)
                        {
                            KeySendSingle(KeysVirtual.BackSpace, vProcessCurrent.WindowHandleMain);
                        }
                        else if (listAppCategory == ListCategory.Search)
                        {
                            await AVActions.DispatcherInvoke(async delegate { await Search_Reset(true); });
                        }
                        else
                        {
                            await AVActions.DispatcherInvoke(async delegate { await QuickLaunchPrompt(); });
                        }

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        Debug.WriteLine("Button: XPressed");

                        KeySendSingle(KeysVirtual.Delete, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: ShoulderLeftPressed");
                        await AVActions.DispatcherInvoke(async delegate
                        {
                            if (grid_Popup_Settings.Visibility == Visibility.Visible)
                            {
                                await SettingsChangeTab(true);
                                KeySendSingle(KeysVirtual.F13, vProcessCurrent.WindowHandleMain);
                            }
                            else if (!Popup_Open_Any())
                            {
                                ListCategory listCategorySetting = (ListCategory)SettingLoad(vConfigurationCtrlUI, "ListAppCategory", typeof(int));
                                ListCategory listCategorySwitch = (ListCategory)CategoryListPreviousWithItems(listCategorySetting, true);
                                await CategoryListChange(listCategorySwitch);
                            }
                        });

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: ShoulderRightPressed");
                        await AVActions.DispatcherInvoke(async delegate
                        {
                            if (grid_Popup_Settings.Visibility == Visibility.Visible)
                            {
                                await SettingsChangeTab(false);
                                KeySendSingle(KeysVirtual.F13, vProcessCurrent.WindowHandleMain);
                            }
                            else if (!Popup_Open_Any())
                            {
                                ListCategory listCategorySetting = (ListCategory)SettingLoad(vConfigurationCtrlUI, "ListAppCategory", typeof(int));
                                ListCategory listCategorySwitch = (ListCategory)CategoryListNextWithItems(listCategorySetting, true);
                                await CategoryListChange(listCategorySwitch);
                            }
                        });

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Show hide menu");
                        if (vFilePickerOpen)
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                if (vFilePickerFolderSelectMode)
                                {
                                    await Popup_Close_FilePicker(true, true);
                                }
                                else
                                {
                                    FilePicker_CheckItem();
                                }
                            });
                        }
                        else if (vTextInputOpen)
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await ValidateSetTextInput();
                            });
                        }
                        else if (Popup_Open_Check(grid_Popup_Manage))
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await SaveEditManageApplication();
                            });
                        }
                        else
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await Popup_ShowHide_MainMenu(false);
                            });
                        }

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        Debug.WriteLine("Button: BackPressed / Showing search");
                        if (vFilePickerOpen)
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await FilePicker_SortFilesFoldersSwitch(false);
                            });
                        }
                        else if (!Popup_Open_Any())
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await CategoryListChange(ListCategory.Search);
                            });
                        }

                        ControllerUsed = true;
                        ControllerDelay750 = true;
                    }
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: ThumbLeftPressed");
                        KeySendSingle(KeysVirtual.Home, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: ThumbRightPressed");
                        KeySendSingle(KeysVirtual.End, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    if (ControllerDelay125)
                    {
                        vControllerDelay_Button = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay250)
                    {
                        vControllerDelay_Button = GetSystemTicksMs() + vControllerDelayTicks250;
                    }
                    else if (ControllerDelay750)
                    {
                        vControllerDelay_Button = GetSystemTicksMs() + vControllerDelayTicks750;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller DPad
        bool Controller_DPadPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_DPad)
                {
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadLeftPressed");

                        KeySendSingle(KeysVirtual.Left, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadUpPressed");

                        KeySendSingle(KeysVirtual.Up, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadRightPressed");

                        KeySendSingle(KeysVirtual.Right, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadDownPressed");

                        KeySendSingle(KeysVirtual.Down, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    if (ControllerDelay125)
                    {
                        vControllerDelay_DPad = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay750)
                    {
                        vControllerDelay_DPad = GetSystemTicksMs() + vControllerDelayTicks750;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller triggers
        async Task<bool> Controller_TriggerPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Trigger)
                {
                    if (ControllerInput.TriggerLeft > 0)
                    {
                        await ListBoxSelectNearCharacter(false);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.TriggerRight > 0)
                    {
                        await ListBoxSelectNearCharacter(true);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    if (ControllerDelay125)
                    {
                        vControllerDelay_Trigger = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay750)
                    {
                        vControllerDelay_Trigger = GetSystemTicksMs() + vControllerDelayTicks750;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller sticks
        bool Controller_StickMovement(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Stick)
                {
                    //Left stick movement
                    if (ControllerInput.ThumbLeftX < -10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Left, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ThumbLeftY > 10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Up, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ThumbLeftX > 10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Right, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ThumbLeftY < -10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Down, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    //Right stick movement
                    if (ControllerInput.ThumbRightX < -10000 && Math.Abs(ControllerInput.ThumbRightY) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Prior, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ThumbRightY > 10000 && Math.Abs(ControllerInput.ThumbRightX) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Prior, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ThumbRightX > 10000 && Math.Abs(ControllerInput.ThumbRightY) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Next, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ThumbRightY < -10000 && Math.Abs(ControllerInput.ThumbRightX) < 13000)
                    {
                        KeySendSingle(KeysVirtual.Next, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    if (ControllerDelay125)
                    {
                        vControllerDelay_Stick = GetSystemTicksMs() + vControllerDelayTicks125;
                    }
                    else if (ControllerDelay750)
                    {
                        vControllerDelay_Stick = GetSystemTicksMs() + vControllerDelayTicks750;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }
    }
}