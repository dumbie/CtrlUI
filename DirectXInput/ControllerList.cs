using ArnoldVinkCode;
using LibraryUsb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryUsb.HidDevices;
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
                //DualShock 3 ScpDriver guid
                Guid EnumerateClass = new Guid("{E2824A09-DBAA-4407-85CA-C8E8FF5F6FFA}");
                IEnumerable<EnumerateInfo> SelectedWinDevice = EnumerateDevices(EnumerateClass);
                foreach (EnumerateInfo EnumDevice in SelectedWinDevice)
                {
                    try
                    {
                        //Get vendor and product hex id
                        string VendorHexId = "0x" + AVFunctions.StringShowAfter(EnumDevice.DevicePath, "vid_", 4).ToLower();
                        string ProductHexId = "0x" + AVFunctions.StringShowAfter(EnumDevice.DevicePath, "pid_", 4).ToLower();

                        //Validate the connected controller
                        if (!ControllerValidate(VendorHexId, ProductHexId, EnumDevice.DevicePath)) { continue; }

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> ProfileList = vDirectControllersProfile.Where(x => x.ProductID.ToLower() == ProductHexId && x.VendorID.ToLower() == VendorHexId);
                        if (!ProfileList.Any())
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

                        //Check if controller is wireless
                        bool ConnectedWireless = EnumDevice.DevicePath.ToLower().Contains("00805f9b34fb");

                        ControllerDetails newController = new ControllerDetails()
                        {
                            Type = "Win",
                            Profile = ProfileList.FirstOrDefault(),
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
                //Get the current hid guid
                HidD_GetHidGuid(ref EnumerateClass);
                IEnumerable<EnumerateInfo> SelectedHidDevice = EnumerateDevices(EnumerateClass);
                foreach (EnumerateInfo EnumDevice in SelectedHidDevice)
                {
                    try
                    {
                        //Check if the device is a game controller
                        if (!EnumDevice.Description.EndsWith("game controller")) { continue; }

                        //Read information from the controller
                        HidDevice FoundHidDevice = new HidDevice(EnumDevice.DevicePath, EnumDevice.Description, EnumDevice.HardwareId);

                        //Get vendor and product hex id
                        string VendorHexId = FoundHidDevice.Attributes.VendorHexId.ToLower();
                        string ProductHexId = FoundHidDevice.Attributes.ProductHexId.ToLower();

                        //Validate the connected controller
                        if (!ControllerValidate(VendorHexId, ProductHexId, EnumDevice.DevicePath)) { continue; }

                        //Get controller product information
                        string ProductNameString = FoundHidDevice.Attributes.ProductName;
                        string VendorNameString = FoundHidDevice.Attributes.VendorName;

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> ProfileList = vDirectControllersProfile.Where(x => x.ProductID.ToLower() == ProductHexId && x.VendorID.ToLower() == VendorHexId);
                        if (!ProfileList.Any())
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

                        //Check if controller is wireless
                        bool ConnectedWireless = FoundHidDevice.DevicePath.ToLower().Contains("00805f9b34fb");

                        ControllerDetails newController = new ControllerDetails()
                        {
                            Type = "Hid",
                            Profile = ProfileList.FirstOrDefault(),
                            DisplayName = ProductNameString,
                            HardwareId = FoundHidDevice.HardwareId,
                            Path = FoundHidDevice.DevicePath,
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