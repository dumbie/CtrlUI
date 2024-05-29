using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkCode.AVInputOutputClass;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        public void Shortcuts_Check()
        {
            try
            {
                Debug.WriteLine("Checking application shortcuts...");

                //Keyboard
                if (!vShortcutsKeyboard.Any(x => x.Name == "LaunchCtrlUI"))
                {
                    ShortcutTriggerKeyboard shortcutTrigger = new ShortcutTriggerKeyboard();
                    shortcutTrigger.Name = "LaunchCtrlUI";
                    shortcutTrigger.Trigger = [KeysVirtual.WindowsLeft, KeysVirtual.None, KeysVirtual.Tilde];
                    vShortcutsKeyboard.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsKeyboard, @"Profiles\User\DirectShortcutsKeyboard.json");
                }

                //Controller
                if (!vShortcutsController.Any(x => x.Name == "LaunchCtrlUI"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "LaunchCtrlUI";
                    shortcutTrigger.Trigger = [ControllerButtons.Guide];
                    shortcutTrigger.Hold = true;
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                //Shared
                if (!vShortcutsController.Any(x => x.Name == "KeyboardPopup"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "KeyboardPopup";
                    shortcutTrigger.Trigger = [ControllerButtons.Guide];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                //Shared
                if (!vShortcutsController.Any(x => x.Name == "AltEnter"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "AltEnter";
                    shortcutTrigger.Trigger = [ControllerButtons.ShoulderRight, ControllerButtons.Start];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                //Shared
                if (!vShortcutsController.Any(x => x.Name == "AltTab"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "AltTab";
                    shortcutTrigger.Trigger = [ControllerButtons.ShoulderLeft, ControllerButtons.Start];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                if (!vShortcutsController.Any(x => x.Name == "CtrlAltDelete"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "CtrlAltDelete";
                    shortcutTrigger.Trigger = [ControllerButtons.Guide, ControllerButtons.Back];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                if (!vShortcutsController.Any(x => x.Name == "MuteOutput"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "MuteOutput";
                    shortcutTrigger.Trigger = [ControllerButtons.Two];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                if (!vShortcutsController.Any(x => x.Name == "MuteInput"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "MuteInput";
                    shortcutTrigger.Trigger = [ControllerButtons.Two];
                    shortcutTrigger.Hold = true;
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                //Shared
                if (!vShortcutsController.Any(x => x.Name == "CaptureImage"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "CaptureImage";
                    shortcutTrigger.Trigger = [ControllerButtons.One];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                if (!vShortcutsController.Any(x => x.Name == "CaptureVideo"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "CaptureVideo";
                    shortcutTrigger.Trigger = [ControllerButtons.One];
                    shortcutTrigger.Hold = true;
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }

                if (!vShortcutsController.Any(x => x.Name == "DisconnectController"))
                {
                    ShortcutTriggerController shortcutTrigger = new ShortcutTriggerController();
                    shortcutTrigger.Name = "DisconnectController";
                    shortcutTrigger.Trigger = [ControllerButtons.Guide, ControllerButtons.Start];
                    vShortcutsController.Add(shortcutTrigger);
                    AVJsonFunctions.JsonSaveObject(vShortcutsController, @"Profiles\User\DirectShortcutsController.json");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to check application shortcuts: " + ex.Message);
            }
        }
    }
}