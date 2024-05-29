using ArnoldVinkCode;
using System;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static DirectXInput.ProfileFunctions;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Save controller button mapping
        bool ControllerMappingSave(ControllerStatus controller)
        {
            try
            {
                //Check if controller mapping is enabled
                if (vMappingControllerStatus == MappingStatus.Mapping)
                {
                    //Store new button mapping in Json controller
                    int buttonMapId = Array.FindIndex(controller.InputCurrent.Buttons, x => x.PressedRaw);
                    if (buttonMapId > -1)
                    {
                        AVActions.DispatcherInvoke(delegate
                        {
                            try
                            {
                                //Fix block dpad and thumb movement
                                string mapNameString = vMappingControllerButton.ToolTip.ToString();
                                Debug.WriteLine("Mapped button " + mapNameString + " to: " + (ControllerButtons)buttonMapId);
                                if (vMappingControllerButton == btn_SetA) { controller.Details.Profile.ButtonA = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetB) { controller.Details.Profile.ButtonB = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetX) { controller.Details.Profile.ButtonX = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetY) { controller.Details.Profile.ButtonY = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetShoulderLeft) { controller.Details.Profile.ButtonShoulderLeft = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetShoulderRight) { controller.Details.Profile.ButtonShoulderRight = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetBack) { controller.Details.Profile.ButtonBack = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetStart) { controller.Details.Profile.ButtonStart = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetGuide) { controller.Details.Profile.ButtonGuide = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThumbLeft) { controller.Details.Profile.ButtonThumbLeft = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThumbRight) { controller.Details.Profile.ButtonThumbRight = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTriggerLeft) { controller.Details.Profile.ButtonTriggerLeft = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTriggerRight) { controller.Details.Profile.ButtonTriggerRight = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetOne) { controller.Details.Profile.ButtonOne = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetTwo) { controller.Details.Profile.ButtonTwo = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetThree) { controller.Details.Profile.ButtonThree = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetFour) { controller.Details.Profile.ButtonFour = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetFive) { controller.Details.Profile.ButtonFive = (ControllerButtons)buttonMapId; }
                                else if (vMappingControllerButton == btn_SetSix) { controller.Details.Profile.ButtonSix = (ControllerButtons)buttonMapId; }
                            }
                            catch { }
                        });

                        //Save changes to Json file
                        JsonSaveObject(controller.Details.Profile, GenerateJsonNameControllerProfile(controller.Details.Profile));

                        //Reset controller button mapping
                        vMappingControllerStatus = MappingStatus.Done;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}