using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInputOutputKeyboard;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

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
                    await Controller_DPadPress(ControllerInput);
                    await Controller_StickMovement(ControllerInput);
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
            bool ControllerDelayShort = false;
            bool ControllerDelayMedium = false;
            bool ControllerDelayLonger = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Button)
                {
                    if (ControllerInput.ButtonA.PressedRaw)
                    {
                        Debug.WriteLine("Button: APressed");

                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                        {
                            FrameworkElement frameworkElement = (FrameworkElement)Keyboard.FocusedElement;
                            if (frameworkElement != null && frameworkElement.GetType() == typeof(TextBox))
                            {
                                //Launch the keyboard controller
                                if (vAppActivated && vControllerAnyConnected())
                                {
                                    await KeyboardControllerHideShow(true);
                                }
                            }
                            else
                            {
                                //Press on the space bar
                                await KeySendSingle(KeysVirtual.Space, vProcessCurrent.MainWindowHandle);
                            }
                        });

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonB.PressedRaw)
                    {
                        Debug.WriteLine("Button: BPressed");

                        if (Popup_Any_Open())
                        {
                            await KeySendSingle(KeysVirtual.Escape, vProcessCurrent.MainWindowHandle);
                        }

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonY.PressedRaw)
                    {
                        Debug.WriteLine("Button: YPressed");

                        if (vTextInputOpen)
                        {
                            Debug.WriteLine("Resetting the text input popup.");
                            await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Reset_TextInput(true, string.Empty); });
                        }
                        else if (vSearchOpen)
                        {
                            Debug.WriteLine("Resetting the search popup.");
                            await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_Reset_Search(true); });
                        }
                        else if (vFilePickerOpen)
                        {
                            await KeySendSingle(KeysVirtual.BackSpace, vProcessCurrent.MainWindowHandle);
                        }
                        else
                        {
                            await AVActions.ActionDispatcherInvokeAsync(async delegate { await QuickActionPrompt(); });
                        }

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonX.PressedRaw)
                    {
                        Debug.WriteLine("Button: XPressed");

                        await KeySendSingle(KeysVirtual.Delete, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayMedium = true;
                    }
                    else if (ControllerInput.ButtonShoulderLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: ShoulderLeftPressed");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                         {
                             if (grid_Popup_Settings.Visibility == Visibility.Visible)
                             {
                                 await SettingsChangeTab(true);
                                 await KeySendSingle(KeysVirtual.F13, vProcessCurrent.MainWindowHandle);
                             }
                             else
                             {
                                 await KeyPressComboAuto(KeysVirtual.Shift, KeysVirtual.Tab);
                             }
                         });

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonShoulderRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: ShoulderRightPressed");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate
                        {
                            if (grid_Popup_Settings.Visibility == Visibility.Visible)
                            {
                                await SettingsChangeTab(false);
                                await KeySendSingle(KeysVirtual.F13, vProcessCurrent.MainWindowHandle);
                            }
                            else
                            {
                                await KeySendSingle(KeysVirtual.Tab, vProcessCurrent.MainWindowHandle);
                            }
                        });

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonBack.PressedRaw)
                    {
                        Debug.WriteLine("Button: BackPressed / Show hide menu");
                        await AVActions.ActionDispatcherInvokeAsync(async delegate { await Popup_ShowHide_MainMenu(false); });

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    else if (ControllerInput.ButtonStart.PressedRaw)
                    {
                        Debug.WriteLine("Button: StartPressed / Show hide search");
                        if (vFilePickerOpen)
                        {
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                FilePicker_SelectItem();
                            });
                        }
                        else
                        {
                            await AVActions.ActionDispatcherInvokeAsync(async delegate
                            { 
                                await Popup_ShowHide_Search(false); 
                            });
                        }

                        ControllerUsed = true;
                        ControllerDelayLonger = true;
                    }
                    else if (ControllerInput.ButtonThumbLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: ThumbLeftPressed");
                        await KeySendSingle(KeysVirtual.Home, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ButtonThumbRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: ThumbRightPressed");
                        await KeySendSingle(KeysVirtual.End, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayMedium)
                    {
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayMediumTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        vControllerDelay_Button = Environment.TickCount + vControllerDelayLongerTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller D-Pad
        async Task<bool> Controller_DPadPress(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLonger = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_DPad)
                {
                    if (ControllerInput.DPadLeft.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadLeftPressed");

                        await KeySendSingle(KeysVirtual.Left, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.DPadUp.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadUpPressed");

                        await KeySendSingle(KeysVirtual.Up, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.DPadRight.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadRightPressed");

                        await KeySendSingle(KeysVirtual.Right, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.DPadDown.PressedRaw)
                    {
                        Debug.WriteLine("Button: DPadDownPressed");

                        await KeySendSingle(KeysVirtual.Down, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_DPad = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        vControllerDelay_DPad = Environment.TickCount + vControllerDelayLongerTicks;
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
            bool ControllerDelayShort = false;
            bool ControllerDelayLonger = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Trigger)
                {
                    //Check if the media popup is opened
                    bool MediaPopupOpen = false;
                    await AVActions.ActionDispatcherInvokeAsync(delegate { MediaPopupOpen = grid_Popup_Media.Visibility == Visibility.Visible; });

                    //Control the volume with controller triggers
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["ShortcutVolume"]) || MediaPopupOpen)
                    {
                        if (ControllerInput.TriggerLeft > 0 && ControllerInput.TriggerRight > 0)
                        {
                            await KeyPressSingleAuto(KeysVirtual.VolumeMute);

                            ControllerUsed = true;
                            ControllerDelayLonger = true;
                        }
                        else if (ControllerInput.TriggerLeft > 0)
                        {
                            await KeyPressSingleAuto(KeysVirtual.VolumeDown);

                            ControllerUsed = true;
                            ControllerDelayShort = true;
                        }
                        else if (ControllerInput.TriggerRight > 0)
                        {
                            await KeyPressSingleAuto(KeysVirtual.VolumeUp);

                            ControllerUsed = true;
                            ControllerDelayShort = true;
                        }
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Trigger = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        vControllerDelay_Trigger = Environment.TickCount + vControllerDelayLongerTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }

        //Process XInput controller sticks
        async Task<bool> Controller_StickMovement(ControllerInput ControllerInput)
        {
            bool ControllerUsed = false;
            bool ControllerDelayShort = false;
            bool ControllerDelayLonger = false;
            try
            {
                if (Environment.TickCount >= vControllerDelay_Stick)
                {
                    //Left stick movement
                    if (ControllerInput.ThumbLeftX < -10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Left, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY > 10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Up, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftX > 10000 && Math.Abs(ControllerInput.ThumbLeftY) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Right, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbLeftY < -10000 && Math.Abs(ControllerInput.ThumbLeftX) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Down, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    //Right stick movement
                    if (ControllerInput.ThumbRightX < -10000 && Math.Abs(ControllerInput.ThumbRightY) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Prior, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbRightY > 10000 && Math.Abs(ControllerInput.ThumbRightX) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Prior, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbRightX > 10000 && Math.Abs(ControllerInput.ThumbRightY) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Next, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }
                    else if (ControllerInput.ThumbRightY < -10000 && Math.Abs(ControllerInput.ThumbRightX) < 13000)
                    {
                        await KeySendSingle(KeysVirtual.Next, vProcessCurrent.MainWindowHandle);

                        ControllerUsed = true;
                        ControllerDelayShort = true;
                    }

                    if (ControllerDelayShort)
                    {
                        vControllerDelay_Stick = Environment.TickCount + vControllerDelayShortTicks;
                    }
                    else if (ControllerDelayLonger)
                    {
                        vControllerDelay_Stick = Environment.TickCount + vControllerDelayLongerTicks;
                    }
                }
            }
            catch { }
            return ControllerUsed;
        }
    }
}