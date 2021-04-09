using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;
using static LibraryUsb.Enumerate;
using static LibraryUsb.NativeMethods_Guid;
using static LibraryUsb.NativeMethods_Hid;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive all the connected Controllers
        async Task ControllerReceiveAllConnected()
        {
            try
            {
                //Add Win Usb Devices
                IEnumerable<EnumerateInfo> SelectedWinDevice = EnumerateDevicesDi(GuidClassScpDS3Driver, true);
                foreach (EnumerateInfo EnumDevice in SelectedWinDevice)
                {
                    try
                    {
                        //Get vendor and product hex id
                        string VendorHexId = "0x" + AVFunctions.StringShowAfter(EnumDevice.DevicePath, "vid_", 4).ToLower();
                        string ProductHexId = "0x" + AVFunctions.StringShowAfter(EnumDevice.DevicePath, "pid_", 4).ToLower();

                        //Validate the connected controller
                        if (!ControllerValidate(VendorHexId, ProductHexId, EnumDevice.DevicePath, string.Empty)) { continue; }

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> profileList = vDirectControllersProfile.Where(x => x.ProductID.ToLower() == ProductHexId && x.VendorID.ToLower() == VendorHexId);
                        if (!profileList.Any())
                        {
                            vDirectControllersProfile.Add(new ControllerProfile()
                            {
                                ProductID = ProductHexId,
                                VendorID = VendorHexId,
                                ProductName = EnumDevice.Description,
                                VendorName = "Unknown"
                            });

                            //Save changes to Json file
                            JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

                            Debug.WriteLine("Added win profile: " + EnumDevice.Description);
                        }
                        ControllerProfile profileController = profileList.FirstOrDefault();

                        //Check if controller is ignored
                        if (profileController.Ignore)
                        {
                            //Debug.WriteLine("Controller is ignored: " + ProductHexId + "/" + VendorHexId);
                            continue;
                        }

                        //Check if controller is wireless
                        bool ConnectedWireless = EnumDevice.DevicePath.ToLower().Contains("00805f9b34fb");

                        ControllerDetails newController = new ControllerDetails()
                        {
                            Type = "Win",
                            Profile = profileController,
                            DisplayName = EnumDevice.Description,
                            Path = EnumDevice.DevicePath,
                            Wireless = ConnectedWireless
                        };

                        //Connect with the controller
                        await ControllerConnect(newController);
                    }
                    catch { }
                }

                //Add Hib Usb Devices
                IEnumerable<EnumerateInfo> SelectedHidDevice = EnumerateDevicesDi(GuidClassHidDevice, true);
                foreach (EnumerateInfo EnumDevice in SelectedHidDevice)
                {
                    try
                    {
                        //Read information from the controller
                        HidDevice foundHidDevice = new HidDevice(EnumDevice.DevicePath, EnumDevice.ModelId, false, true);

                        //Check if device is a gamepad or joystick
                        bool genericGamePad = foundHidDevice.Capabilities.UsageGeneric == (short)HID_USAGE_GENERIC_DESKTOP_PAGE.HID_USAGE_GENERIC_GAMEPAD;
                        bool genericJoystick = foundHidDevice.Capabilities.UsageGeneric == (short)HID_USAGE_GENERIC_DESKTOP_PAGE.HID_USAGE_GENERIC_JOYSTICK;
                        if (!genericGamePad && !genericJoystick) { continue; }

                        //Get vendor and product hex id
                        string VendorHexId = foundHidDevice.Attributes.VendorHexId.ToLower();
                        string ProductHexId = foundHidDevice.Attributes.ProductHexId.ToLower();

                        //Validate the connected controller
                        if (!ControllerValidate(VendorHexId, ProductHexId, EnumDevice.DevicePath, foundHidDevice.Attributes.SerialNumber)) { continue; }

                        //Get controller product information
                        string ProductNameString = foundHidDevice.Attributes.ProductName;
                        string VendorNameString = foundHidDevice.Attributes.VendorName;

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> profileList = vDirectControllersProfile.Where(x => x.ProductID.ToLower() == ProductHexId && x.VendorID.ToLower() == VendorHexId);
                        if (!profileList.Any())
                        {
                            ControllerProfile NewController = new ControllerProfile()
                            {
                                ProductID = ProductHexId,
                                VendorID = VendorHexId,
                                ProductName = ProductNameString,
                                VendorName = VendorNameString
                            };

                            vDirectControllersProfile.Add(NewController);

                            //Save changes to Json file
                            JsonSaveObject(vDirectControllersProfile, "DirectControllersProfile");

                            Debug.WriteLine("Added hid profile: " + ProductNameString + " (" + VendorNameString + ")");
                        }
                        ControllerProfile profileController = profileList.FirstOrDefault();

                        //Check if controller is ignored
                        if (profileController.Ignore)
                        {
                            //Debug.WriteLine("Controller is ignored: " + ProductHexId + "/" + VendorHexId);
                            continue;
                        }

                        //Check if controller is wireless
                        bool ConnectedWireless = foundHidDevice.DevicePath.ToLower().Contains("00805f9b34fb");

                        ControllerDetails newController = new ControllerDetails()
                        {
                            Type = "Hid",
                            Profile = profileController,
                            DisplayName = ProductNameString,
                            ModelId = foundHidDevice.ModelId,
                            Path = foundHidDevice.DevicePath,
                            Wireless = ConnectedWireless
                        };

                        //Connect with the controller
                        await ControllerConnect(newController);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed adding new controller: " + ex.Message);
            }
        }
    }
}