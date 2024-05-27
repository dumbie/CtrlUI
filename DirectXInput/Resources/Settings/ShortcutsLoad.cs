using System;
using System.Diagnostics;
using System.Linq;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        void Shortcuts_Load()
        {
            try
            {
                Debug.WriteLine("Loading application shortcuts...");

                //Keyboard
                keyboard_LaunchCtrlUI.Set(vShortcutsKeyboard.FirstOrDefault(x => x.Name == keyboard_LaunchCtrlUI.TriggerName));

                //Controller
                controller_FakeGuideButton.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_FakeGuideButton.TriggerName));
                controller_DisconnectController.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_DisconnectController.TriggerName));
                controller_LaunchCtrlUI.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_LaunchCtrlUI.TriggerName));
                controller_KeyboardPopup.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_KeyboardPopup.TriggerName));
                controller_AltTab.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_AltTab.TriggerName));
                controller_AltEnter.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_AltEnter.TriggerName));
                controller_CtrlAltDelete.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_CtrlAltDelete.TriggerName));
                controller_MuteOutput.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_MuteOutput.TriggerName));
                controller_MuteInput.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_MuteInput.TriggerName));
                controller_CaptureImage.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_CaptureImage.TriggerName));
                controller_CaptureVideo.Set(vShortcutsController.FirstOrDefault(x => x.Name == controller_CaptureVideo.TriggerName));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load application shortcuts: " + ex.Message);
            }
        }
    }
}