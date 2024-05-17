using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Lookup controller button mapping
        bool ControllerMappingLookup(ControllerStatus controller)
        {
            try
            {
                //Buttons (A, B, X, Y)
                if (controller.Details.Profile.ButtonA == null) { controller.InputCurrent.ButtonA.PressedRaw = controller.InputCurrent.ButtonPressStatus[0]; }
                else if (controller.Details.Profile.ButtonA != -1) { controller.InputCurrent.ButtonA.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonA.Value]; }

                if (controller.Details.Profile.ButtonB == null) { controller.InputCurrent.ButtonB.PressedRaw = controller.InputCurrent.ButtonPressStatus[1]; }
                else if (controller.Details.Profile.ButtonB != -1) { controller.InputCurrent.ButtonB.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonB.Value]; }

                if (controller.Details.Profile.ButtonX == null) { controller.InputCurrent.ButtonX.PressedRaw = controller.InputCurrent.ButtonPressStatus[2]; }
                else if (controller.Details.Profile.ButtonX != -1) { controller.InputCurrent.ButtonX.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonX.Value]; }

                if (controller.Details.Profile.ButtonY == null) { controller.InputCurrent.ButtonY.PressedRaw = controller.InputCurrent.ButtonPressStatus[3]; }
                else if (controller.Details.Profile.ButtonY != -1) { controller.InputCurrent.ButtonY.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonY.Value]; }

                //Buttons (Shoulders, Triggers, Thumbs, Back, Start)
                if (controller.Details.Profile.ButtonShoulderLeft == null) { controller.InputCurrent.ButtonShoulderLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[100]; }
                else if (controller.Details.Profile.ButtonShoulderLeft != -1) { controller.InputCurrent.ButtonShoulderLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonShoulderLeft.Value]; }

                if (controller.Details.Profile.ButtonShoulderRight == null) { controller.InputCurrent.ButtonShoulderRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[101]; }
                else if (controller.Details.Profile.ButtonShoulderRight != -1) { controller.InputCurrent.ButtonShoulderRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonShoulderRight.Value]; }

                if (controller.Details.Profile.ButtonTriggerLeft == null) { controller.InputCurrent.ButtonTriggerLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[102]; }
                else if (controller.Details.Profile.ButtonTriggerLeft != -1) { controller.InputCurrent.ButtonTriggerLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTriggerLeft.Value]; }

                if (controller.Details.Profile.ButtonTriggerRight == null) { controller.InputCurrent.ButtonTriggerRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[103]; }
                else if (controller.Details.Profile.ButtonTriggerRight != -1) { controller.InputCurrent.ButtonTriggerRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTriggerRight.Value]; }

                if (controller.Details.Profile.ButtonThumbLeft == null) { controller.InputCurrent.ButtonThumbLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[104]; }
                else if (controller.Details.Profile.ButtonThumbLeft != -1) { controller.InputCurrent.ButtonThumbLeft.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonThumbLeft.Value]; }

                if (controller.Details.Profile.ButtonThumbRight == null) { controller.InputCurrent.ButtonThumbRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[105]; }
                else if (controller.Details.Profile.ButtonThumbRight != -1) { controller.InputCurrent.ButtonThumbRight.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonThumbRight.Value]; }

                if (controller.Details.Profile.ButtonBack == null) { controller.InputCurrent.ButtonBack.PressedRaw = controller.InputCurrent.ButtonPressStatus[106]; }
                else if (controller.Details.Profile.ButtonBack != -1) { controller.InputCurrent.ButtonBack.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonBack.Value]; }

                if (controller.Details.Profile.ButtonStart == null) { controller.InputCurrent.ButtonStart.PressedRaw = controller.InputCurrent.ButtonPressStatus[107]; }
                else if (controller.Details.Profile.ButtonStart != -1) { controller.InputCurrent.ButtonStart.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonStart.Value]; }

                //Buttons (Guide and others)
                if (controller.Details.Profile.ButtonGuide == null) { controller.InputCurrent.ButtonGuide.PressedRaw = controller.InputCurrent.ButtonPressStatus[200]; }
                else if (controller.Details.Profile.ButtonGuide != -1) { controller.InputCurrent.ButtonGuide.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonGuide.Value]; }

                if (controller.Details.Profile.ButtonOne == null) { controller.InputCurrent.ButtonOne.PressedRaw = controller.InputCurrent.ButtonPressStatus[201]; }
                else if (controller.Details.Profile.ButtonOne != -1) { controller.InputCurrent.ButtonOne.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonOne.Value]; }

                if (controller.Details.Profile.ButtonTwo == null) { controller.InputCurrent.ButtonTwo.PressedRaw = controller.InputCurrent.ButtonPressStatus[202]; }
                else if (controller.Details.Profile.ButtonTwo != -1) { controller.InputCurrent.ButtonTwo.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonTwo.Value]; }

                if (controller.Details.Profile.ButtonThree == null) { controller.InputCurrent.ButtonThree.PressedRaw = controller.InputCurrent.ButtonPressStatus[203]; }
                else if (controller.Details.Profile.ButtonThree != -1) { controller.InputCurrent.ButtonThree.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonThree.Value]; }

                if (controller.Details.Profile.ButtonFour == null) { controller.InputCurrent.ButtonFour.PressedRaw = controller.InputCurrent.ButtonPressStatus[204]; }
                else if (controller.Details.Profile.ButtonFour != -1) { controller.InputCurrent.ButtonFour.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonFour.Value]; }

                if (controller.Details.Profile.ButtonFive == null) { controller.InputCurrent.ButtonFive.PressedRaw = controller.InputCurrent.ButtonPressStatus[205]; }
                else if (controller.Details.Profile.ButtonFive != -1) { controller.InputCurrent.ButtonFive.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonFive.Value]; }

                if (controller.Details.Profile.ButtonSix == null) { controller.InputCurrent.ButtonSix.PressedRaw = controller.InputCurrent.ButtonPressStatus[206]; }
                else if (controller.Details.Profile.ButtonSix != -1) { controller.InputCurrent.ButtonSix.PressedRaw = controller.InputCurrent.ButtonPressStatus[controller.Details.Profile.ButtonSix.Value]; }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to lookup controller " + controller.NumberId + " button mapping: " + ex.Message);
                return false;
            }
        }

        //Save controller button mapping
        bool ControllerMappingSave(ControllerStatus Controller)
        {
            try
            {
                //Check if controller mapping is enabled
                if (vMappingControllerStatus == MappingStatus.Mapping)
                {
                    //Store new button mapping in Json controller
                    int buttonMapId = Array.FindIndex(Controller.InputCurrent.ButtonPressStatus, ButtonPressed => ButtonPressed);
                    if (buttonMapId != -1)
                    {
                        AVActions.DispatcherInvoke(delegate
                        {
                            try
                            {
                                string mapNameString = vMappingControllerButton.ToolTip.ToString();
                                Debug.WriteLine("Mapped button " + mapNameString + " to: " + buttonMapId);
                                if (vMappingControllerButton == btn_SetA) { Controller.Details.Profile.ButtonA = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetB) { Controller.Details.Profile.ButtonB = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetX) { Controller.Details.Profile.ButtonX = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetY) { Controller.Details.Profile.ButtonY = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetShoulderLeft) { Controller.Details.Profile.ButtonShoulderLeft = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetShoulderRight) { Controller.Details.Profile.ButtonShoulderRight = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetBack) { Controller.Details.Profile.ButtonBack = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetStart) { Controller.Details.Profile.ButtonStart = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetGuide) { Controller.Details.Profile.ButtonGuide = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThumbLeft) { Controller.Details.Profile.ButtonThumbLeft = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThumbRight) { Controller.Details.Profile.ButtonThumbRight = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTriggerLeft) { Controller.Details.Profile.ButtonTriggerLeft = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTriggerRight) { Controller.Details.Profile.ButtonTriggerRight = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetOne) { Controller.Details.Profile.ButtonOne = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTwo) { Controller.Details.Profile.ButtonTwo = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThree) { Controller.Details.Profile.ButtonThree = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetFour) { Controller.Details.Profile.ButtonFour = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetFive) { Controller.Details.Profile.ButtonFive = buttonMapId; }
                                else if (vMappingControllerButton == btn_SetSix) { Controller.Details.Profile.ButtonSix = buttonMapId; }
                            }
                            catch { }
                        });

                        //Save changes to Json file
                        JsonSaveObject(Controller.Details.Profile, GenerateJsonNameControllerProfile(Controller.Details.Profile));

                        //Reset controller button mapping
                        vMappingControllerStatus = MappingStatus.Done;
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}