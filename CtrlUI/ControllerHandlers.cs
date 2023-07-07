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

                        if (vTextInputOpen)
                        {
                            Debug.WriteLine("Resetting the text input popup.");
                            await AVActions.DispatcherInvoke(async delegate { await Popup_Reset_TextInput(true, string.Empty); });
                        }
                        else if (vFilePickerOpen)
                        {
                            KeySendSingle(KeysVirtual.BackSpace, vProcessCurrent.WindowHandleMain);
                        }
                        else if (vCurrentListCategory == ListCategory.Search)
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
                                ListCategory listCategorySwitch = (ListCategory)CategoryListPreviousWithItems(vCurrentListCategory, true);
                                await CategoryListChange(listCategorySwitch);
                                KeySendSingle(KeysVirtual.F13, vProcessCurrent.WindowHandleMain);
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
                                ListCategory listCategorySwitch = (ListCategory)CategoryListNextWithItems(vCurrentListCategory, true);
                                await CategoryListChange(listCategorySwitch);
                                KeySendSingle(KeysVirtual.F13, vProcessCurrent.WindowHandleMain);
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
            bool ControllerDelay25 = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_DPad)
                {
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadLeftPressed");

                        KeySendSingle(KeysVirtual.ArrowLeft, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.DPadLeft.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadUpPressed");

                        KeySendSingle(KeysVirtual.ArrowUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.DPadUp.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadRightPressed");

                        KeySendSingle(KeysVirtual.ArrowRight, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.DPadRight.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadDownPressed");

                        KeySendSingle(KeysVirtual.ArrowDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.DPadDown.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }

                    if (ControllerDelay25)
                    {
                        vControllerDelay_DPad = GetSystemTicksMs() + vControllerDelayTicks25;
                    }
                    else if (ControllerDelay125)
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
            bool ControllerDelay25 = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Stick)
                {
                    //Left stick movement
                    if (ControllerInput.ButtonThumbLeftLeft.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowLeft, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.ButtonThumbLeftLeft.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.ButtonThumbLeftUp.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.ButtonThumbLeftUp.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.ButtonThumbLeftRight.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowRight, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.ButtonThumbLeftRight.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.ButtonThumbLeftDown.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.ButtonThumbLeftDown.PressTimeCurrent > 200)
                        {
                            ControllerDelay25 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }

                    //Right stick movement
                    if (ControllerInput.ButtonThumbRightLeft.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonThumbRightUp.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonThumbRightRight.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.ButtonThumbRightDown.PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    if (ControllerDelay25)
                    {
                        vControllerDelay_Stick = GetSystemTicksMs() + vControllerDelayTicks25;
                    }
                    else if (ControllerDelay125)
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