using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.SettingsNotify;

namespace DirectXInput
{
    partial class WindowMain
    {
        void Shortcuts_Save()
        {
            try
            {
                Debug.WriteLine("Saving application shortcuts...");

                //Keyboard
                keyboard_LaunchCtrlUI.TriggerChanged += Shortcut_Keyboard_TriggerChanged;

                //Controller
                controller_DisconnectController.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_LaunchCtrlUI.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_KeyboardPopup.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_AltTab.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_AltEnter.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_CtrlAltDelete.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_MuteOutput.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_MuteInput.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_CaptureImage.TriggerChanged += Shortcut_Controller_TriggerChanged;
                controller_CaptureVideo.TriggerChanged += Shortcut_Controller_TriggerChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save application shortcuts: " + ex.Message);
            }
        }

        async void Shortcut_Keyboard_TriggerChanged(ShortcutTriggerKeyboard triggers)
        {
            try
            {
                if (vShortcutsKeyboard.ListReplaceFirstItem(x => x.Name == triggers.Name, triggers))
                {
                    JsonSaveObject(vShortcutsKeyboard, @"Profiles\User\DirectShortcutsKeyboard.json");
                    await NotifyCtrlUISettingChanged("Shortcut");
                }
            }
            catch { }
        }

        async void Shortcut_Controller_TriggerChanged(ShortcutTriggerController triggers)
        {
            try
            {
                if (vShortcutsController.ListReplaceFirstItem(x => x.Name == triggers.Name, triggers))
                {
                    JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                    await NotifyCtrlUISettingChanged("Shortcut");
                }
            }
            catch { }
        }
    }
}