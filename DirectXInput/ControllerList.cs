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

                        //Check if the controller is in ignore list
                        bool controllerIgnored = false;
                        foreach (ControllerSupported ignoreCheck in vDirectControllersIgnored)
                        {
                            string filterVendor = ignoreCheck.VendorID.ToLower();
                            string[] filterProducts = ignoreCheck.ProductIDs.Select(x => x.ToLower()).ToArray();
                            if (filterVendor == VendorHexId.ToLower() && filterProducts.Any(ProductHexId.ToLower().Contains))
                            {
                                Debug.WriteLine("Controller is on ignore list: " + EnumDevice.Description + "/" + VendorHexId + "/" + ProductHexId);
                                controllerIgnored = true;
                            }
                        }
                        if (controllerIgnored) { continue; }

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> ProfileList = vDirectControllersProfile.Where(x => x.ProductID == ProductHexId && x.VendorID == VendorHexId);
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
                        else
                        {
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
                            await ConnectController(newController);
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

                        //Check if the controller is in ignore list
                        bool controllerIgnored = false;
                        foreach (ControllerSupported ignoreCheck in vDirectControllersIgnored)
                        {
                            string filterVendor = ignoreCheck.VendorID.ToLower();
                            string[] filterProducts = ignoreCheck.ProductIDs.Select(x => x.ToLower()).ToArray();
                            if (filterVendor == VendorHexId.ToLower() && filterProducts.Any(ProductHexId.ToLower().Contains))
                            {
                                Debug.WriteLine("Controller is on ignore list: " + EnumDevice.Description + "/" + VendorHexId + "/" + ProductHexId);
                                controllerIgnored = true;
                            }
                        }
                        if (controllerIgnored) { continue; }

                        //Filter the controller by id and description
                        if (ProductHexId == "0x0000" && VendorHexId == "0x0000") { continue; } //Unknown
                        if (ProductHexId == "0x028e" && VendorHexId == "0x045e") { continue; } //Xbox 360

                        //Get controller product information
                        string ProductNameString = FoundHidDevice.Attributes.ProductName;
                        string VendorNameString = FoundHidDevice.Attributes.VendorName;

                        //Create new Json controller profile if it doesnt exist
                        IEnumerable<ControllerProfile> ProfileList = vDirectControllersProfile.Where(x => x.ProductID == ProductHexId && x.VendorID == VendorHexId);
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
                        else
                        {
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
                            await ConnectController(newController);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex) { Debug.WriteLine("Failed adding new controller: " + ex.Message); }
        }
    }
}