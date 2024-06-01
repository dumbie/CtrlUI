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
                    int mappingIndex = Array.FindIndex(controller.InputCurrent.Buttons, x => x.PressedRaw);
                    ControllerButtons mappingButton = (ControllerButtons)mappingIndex;

                    //Check valid mapping buttons
                    if (mappingIndex == -1 || mappingButton == ControllerButtons.None)
                    {
                        //Debug.WriteLine("No mapping button: " + mappingIndex + "/" + mappingButton);
                        return false;
                    }
                    else if (mappingButton == ControllerButtons.DPadLeft || mappingButton == ControllerButtons.DPadRight || mappingButton == ControllerButtons.DPadUp || mappingButton == ControllerButtons.DPadDown)
                    {
                        Debug.WriteLine("Invalid mapping button: " + mappingButton);
                        return false;
                    }
                    else if (mappingButton == ControllerButtons.ThumbLeftLeft || mappingButton == ControllerButtons.ThumbLeftRight || mappingButton == ControllerButtons.ThumbLeftUp || mappingButton == ControllerButtons.ThumbLeftDown)
                    {
                        Debug.WriteLine("Invalid mapping button: " + mappingButton);
                        return false;
                    }
                    else if (mappingButton == ControllerButtons.ThumbRightLeft || mappingButton == ControllerButtons.ThumbRightRight || mappingButton == ControllerButtons.ThumbRightUp || mappingButton == ControllerButtons.ThumbRightDown)
                    {
                        Debug.WriteLine("Invalid mapping button: " + mappingButton);
                        return false;
                    }

                    AVActions.DispatcherInvoke(delegate
                    {
                        try
                        {
                            string mapNameString = vMappingControllerButton.ToolTip.ToString();
                            Debug.WriteLine("Mapped button " + mapNameString + " to: " + mappingButton);
                            if (vMappingControllerButton == btn_SetA) { controller.Details.Profile.ButtonA = mappingButton; }
                            else if (vMappingControllerButton == btn_SetB) { controller.Details.Profile.ButtonB = mappingButton; }
                            else if (vMappingControllerButton == btn_SetX) { controller.Details.Profile.ButtonX = mappingButton; }
                            else if (vMappingControllerButton == btn_SetY) { controller.Details.Profile.ButtonY = mappingButton; }
                            else if (vMappingControllerButton == btn_SetShoulderLeft) { controller.Details.Profile.ButtonShoulderLeft = mappingButton; }
                            else if (vMappingControllerButton == btn_SetShoulderRight) { controller.Details.Profile.ButtonShoulderRight = mappingButton; }
                            else if (vMappingControllerButton == btn_SetBack) { controller.Details.Profile.ButtonBack = mappingButton; }
                            else if (vMappingControllerButton == btn_SetStart) { controller.Details.Profile.ButtonStart = mappingButton; }
                            else if (vMappingControllerButton == btn_SetGuide) { controller.Details.Profile.ButtonGuide = mappingButton; }
                            else if (vMappingControllerButton == btn_SetThumbLeft) { controller.Details.Profile.ButtonThumbLeft = mappingButton; }
                            else if (vMappingControllerButton == btn_SetThumbRight) { controller.Details.Profile.ButtonThumbRight = mappingButton; }
                            else if (vMappingControllerButton == btn_SetTriggerLeft) { controller.Details.Profile.ButtonTriggerLeft = mappingButton; }
                            else if (vMappingControllerButton == btn_SetTriggerRight) { controller.Details.Profile.ButtonTriggerRight = mappingButton; }
                            else if (vMappingControllerButton == btn_SetOne) { controller.Details.Profile.ButtonOne = mappingButton; }
                            else if (vMappingControllerButton == btn_SetTwo) { controller.Details.Profile.ButtonTwo = mappingButton; }
                            else if (vMappingControllerButton == btn_SetThree) { controller.Details.Profile.ButtonThree = mappingButton; }
                            else if (vMappingControllerButton == btn_SetFour) { controller.Details.Profile.ButtonFour = mappingButton; }
                            else if (vMappingControllerButton == btn_SetFive) { controller.Details.Profile.ButtonFive = mappingButton; }
                            else if (vMappingControllerButton == btn_SetSix) { controller.Details.Profile.ButtonSix = mappingButton; }
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
            catch { }
            return false;
        }
    }
}