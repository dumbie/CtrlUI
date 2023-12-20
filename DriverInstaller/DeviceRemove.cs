using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ArnoldVinkCode.AVDevices.Enumerate;
using static ArnoldVinkCode.AVDevices.Interop;
using static LibraryUsb.NativeMethods_Guid;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        void RemoveUnusedScpVirtualBus()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesSetupApi(GuidClassScpVirtualBus, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            DeviceRemove(GuidClassScpVirtualBus, device.DeviceInstanceId);
                        }
                        catch { }
                    }
                    TextBoxAppend(enumerateInfoList.Count + "x unused ScpVirtualBus removed.");
                }
                else
                {
                    Debug.WriteLine("No unused ScpVirtualBus found.");
                }
            }
            catch { }
        }

        void RemoveUnusedVigemVirtualBus()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesSetupApi(GuidClassVigemVirtualBus, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            DeviceRemove(GuidClassVigemVirtualBus, device.DeviceInstanceId);
                        }
                        catch { }
                    }
                    TextBoxAppend(enumerateInfoList.Count + "x unused VigemVirtualBus removed.");
                }
                else
                {
                    Debug.WriteLine("No unused VigemVirtualBus found.");
                }
            }
            catch { }
        }

        void RemoveUnusedXboxControllers()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesSetupApi(GuidClassX360Controller, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            DeviceRemove(GuidClassX360Controller, device.DeviceInstanceId);
                        }
                        catch { }
                    }
                    TextBoxAppend(enumerateInfoList.Count + "x unused Xbox controller removed.");
                }
                else
                {
                    Debug.WriteLine("No unused Xbox controllers found.");
                }
            }
            catch { }
        }

        void RemoveUnusedDS3Controllers()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesSetupApi(GuidClassScpDS3Driver, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            DeviceRemove(GuidClassScpDS3Driver, device.DeviceInstanceId);
                        }
                        catch { }
                    }
                    TextBoxAppend(enumerateInfoList.Count + "x unused DualShock 3 controller removed.");
                }
                else
                {
                    Debug.WriteLine("No unused DualShock 3 controllers found.");
                }
            }
            catch { }
        }

        void RemoveUnusedFakerInputDevices()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesSetupApi(GuidClassFakerInputDevice, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            DeviceRemove(GuidClassFakerInputDevice, device.DeviceInstanceId);
                        }
                        catch { }
                    }
                    TextBoxAppend(enumerateInfoList.Count + "x unused FakerInput devices removed.");
                }
                else
                {
                    Debug.WriteLine("No unused FakerInput devices found.");
                }
            }
            catch { }
        }
    }
}