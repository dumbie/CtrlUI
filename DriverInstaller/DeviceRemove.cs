using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static LibraryUsb.DeviceManager;
using static LibraryUsb.Enumerate;
using static LibraryUsb.NativeMethods_Guid;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        void RemoveUnusedScpVirtualBus()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesDi(GuidClassScpVirtualBus, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            string DeviceInstanceId = ConvertPathToInstanceId(device.DevicePath);
                            DeviceRemove(GuidClassScpVirtualBus, DeviceInstanceId);
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
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesDi(GuidClassVigemVirtualBus, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            string DeviceInstanceId = ConvertPathToInstanceId(device.DevicePath);
                            DeviceRemove(GuidClassVigemVirtualBus, DeviceInstanceId);
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
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesDi(GuidClassX360Controller, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            string DeviceInstanceId = ConvertPathToInstanceId(device.DevicePath);
                            DeviceRemove(GuidClassX360Controller, DeviceInstanceId);
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
                List<EnumerateInfo> enumerateInfoList = EnumerateDevicesDi(GuidClassScpDS3Driver, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            string DeviceInstanceId = ConvertPathToInstanceId(device.DevicePath);
                            DeviceRemove(GuidClassScpDS3Driver, DeviceInstanceId);
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
    }
}