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
        //Find all the Hid Controllers
        async Task ReceiveAllControllers()
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
                        string VendorHexId = "0x" + AVFunctions.StringShowAfter(EnumDevice.DevicePath, "vid_", 4);
                        string ProductHexId = "0x" + AVFunctions.StringShowAfter(EnumDevice.DevicePath, "pid_", 4);

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> ProfileList = List_ControllerProfile.Where(x => x.ProductID == ProductHexId && x.VendorID == VendorHexId);
                        if (!ProfileList.Any())
                        {
                            List_ControllerProfile.Add(new ControllerProfile()
                            {
                                ProductID = ProductHexId,
                                VendorID = VendorHexId,
                                ProductName = EnumDevice.Description,
                                VendorName = "Unknown"
                            });

                            //Save changes to Json file
                            JsonSaveControllerProfile();

                            Debug.WriteLine("Added win profile: " + EnumDevice.Description);
                        }
                        else
                        {
                            //Check if controller is wireless
                            bool ConnectedWireless = EnumDevice.DevicePath.ToLower().Contains("00805f9b34fb");

                            ControllerConnected NewController = new ControllerConnected()
                            {
                                Type = "Win",
                                Profile = ProfileList.FirstOrDefault(),
                                DisplayName = EnumDevice.Description,
                                Path = EnumDevice.DevicePath,
                                Wireless = ConnectedWireless
                            };

                            await AutoConnectController(NewController);
                        }
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

                        //Filter the controller by id and description
                        if (ProductHexId == "0x0000" && VendorHexId == "0x0000") { continue; } //Unknown
                        if (ProductHexId == "0x028e" && VendorHexId == "0x045e") { continue; } //Xbox 360

                        //Get controller product information
                        string ProductNameString = FoundHidDevice.Attributes.ProductName;
                        string VendorNameString = FoundHidDevice.Attributes.VendorName;

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> ProfileList = List_ControllerProfile.Where(x => x.ProductID == ProductHexId && x.VendorID == VendorHexId);
                        if (!ProfileList.Any())
                        {
                            ControllerProfile NewController = new ControllerProfile()
                            {
                                ProductID = ProductHexId,
                                VendorID = VendorHexId,
                                ProductName = ProductNameString,
                                VendorName = VendorNameString
                            };

                            List_ControllerProfile.Add(NewController);

                            //Save changes to Json file
                            JsonSaveControllerProfile();

                            Debug.WriteLine("Added hid profile: " + ProductNameString + " (" + VendorNameString + ")");
                        }
                        else
                        {
                            //Check if controller is wireless
                            bool ConnectedWireless = FoundHidDevice.DevicePath.ToLower().Contains("00805f9b34fb");

                            ControllerConnected NewController = new ControllerConnected()
                            {
                                Type = "Hid",
                                Profile = ProfileList.FirstOrDefault(),
                                DisplayName = ProductNameString,
                                HardwareId = FoundHidDevice.HardwareId,
                                Path = FoundHidDevice.DevicePath,
                                Wireless = ConnectedWireless
                            };

                            await AutoConnectController(NewController);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex) { Debug.WriteLine("Failed adding new controller: " + ex.Message); }
        }
    }
}