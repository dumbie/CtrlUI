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
using static LibraryShared.ControllerTimings;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Process controller input
        public async Task ControllerInteraction(ControllerInput controllerInput)
        {
            try
            {
                if (!vAppMinimized && vAppActivated)
                {
                    Controller_DPadPress(controllerInput);
                    Controller_StickMovement(controllerInput);
                    await Controller_ButtonPress(controllerInput);
                    await Controller_TriggerPress(controllerInput);
                }
            }
            catch { }
        }

        //Process active XInput controller buttons
        async Task<bool> Controller_ButtonPress(ControllerInput controllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay250 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Button)
                {
                    if (controllerInput.Buttons[(byte)ControllerButtons.A].PressedRaw)
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
                    else if (controllerInput.Buttons[(byte)ControllerButtons.B].PressedRaw)
                    {
                        Debug.WriteLine("Button: BPressed");

                        if (Popup_Open_Any())
                        {
                            KeySendSingle(KeysVirtual.Escape, vProcessCurrent.WindowHandleMain);
                        }
                        else
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await Popup_Show_Sorting();
                            });
                        }

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (controllerInput.Buttons[(byte)ControllerButtons.Y].PressedRaw)
                    {
                        Debug.WriteLine("Button: YPressed");

                        if (vTextInputOpen)
                        {
                            Debug.WriteLine("Resetting the text input popup.");
                            await AVActions.DispatcherInvoke(async delegate { await Popup_Reset_TextInput(true, string.Empty); });
                        }
                        else if (vContentInformationOpen)
                        {
                            ContentInformationSave();
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
                            async Task TaskAction()
                            {
                                try
                                {
                                    await AVActions.DispatcherInvoke(async delegate
                                    {
                                        await QuickLaunchPrompt();
                                    });
                                }
                                catch { }
                            }
                            AVActions.TaskStartBackground(TaskAction);
                        }

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (controllerInput.Buttons[(byte)ControllerButtons.X].PressedRaw)
                    {
                        Debug.WriteLine("Button: XPressed");

                        KeySendSingle(KeysVirtual.Delete, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay250 = true;
                    }
                    else if (controllerInput.Buttons[(byte)ControllerButtons.ShoulderLeft].PressedRaw)
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
                    else if (controllerInput.Buttons[(byte)ControllerButtons.ShoulderRight].PressedRaw)
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
                    else if (controllerInput.Buttons[(byte)ControllerButtons.Start].PressedRaw)
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
                            AVActions.DispatcherInvoke(delegate
                            {
                                ValidateSetTextInput();
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
                    else if (controllerInput.Buttons[(byte)ControllerButtons.Back].PressedRaw)
                    {
                        Debug.WriteLine("Button: BackPressed / Showing search");
                        if (vFilePickerOpen)
                        {
                            await AVActions.DispatcherInvoke(async delegate
                            {
                                await Popup_Show_Sorting();
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
                    else if (controllerInput.Buttons[(byte)ControllerButtons.ThumbLeft].PressedRaw)
                    {
                        Debug.WriteLine("Button: ThumbLeftPressed");
                        KeySendSingle(KeysVirtual.Home, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (controllerInput.Buttons[(byte)ControllerButtons.ThumbRight].PressedRaw)
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
            bool ControllerDelay30 = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_DPad)
                {
                    if (ControllerInput.Buttons[(byte)ControllerButtons.DPadLeft].PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadLeftPressed");

                        KeySendSingle(KeysVirtual.ArrowLeft, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.DPadLeft].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.DPadUp].PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadUpPressed");

                        KeySendSingle(KeysVirtual.ArrowUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.DPadUp].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.DPadRight].PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadRightPressed");

                        KeySendSingle(KeysVirtual.ArrowRight, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.DPadRight].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.DPadDown].PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadDownPressed");

                        KeySendSingle(KeysVirtual.ArrowDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.DPadDown].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }

                    if (ControllerDelay30)
                    {
                        vControllerDelay_DPad = GetSystemTicksMs() + vControllerDelayTicks30;
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
            bool ControllerDelay30 = false;
            bool ControllerDelay125 = false;
            bool ControllerDelay750 = false;
            try
            {
                if (GetSystemTicksMs() >= vControllerDelay_Stick)
                {
                    //Left stick movement
                    if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftLeft].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowLeft, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftLeft].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftUp].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftUp].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftRight].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowRight, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftRight].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftDown].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.ArrowDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;

                        //Check navigation delay
                        if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbLeftDown].PressTimeCurrent > vControllerButtonPressTap)
                        {
                            ControllerDelay30 = true;
                        }
                        else
                        {
                            ControllerDelay125 = true;
                        }
                    }

                    //Right stick movement
                    if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbRightLeft].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbRightUp].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageUp, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbRightRight].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }
                    else if (ControllerInput.Buttons[(byte)ControllerButtons.ThumbRightDown].PressedRaw)
                    {
                        KeySendSingle(KeysVirtual.PageDown, vProcessCurrent.WindowHandleMain);

                        ControllerUsed = true;
                        ControllerDelay125 = true;
                    }

                    if (ControllerDelay30)
                    {
                        vControllerDelay_Stick = GetSystemTicksMs() + vControllerDelayTicks30;
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