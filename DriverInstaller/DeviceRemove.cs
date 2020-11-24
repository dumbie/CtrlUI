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
        void RemoveGhostScpVirtualBus()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevices(GuidClassScpVirtualBus, false);
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
                    TextBoxAppend(enumerateInfoList.Count + "x ghost ScpVirtualBus removed.");
                }
                else
                {
                    Debug.WriteLine("No ghost ScpVirtualBus found.");
                    TextBoxAppend("No ghost ScpVirtualBus found.");
                }
            }
            catch { }
        }

        void RemoveGhostHidGuardian()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevices(GuidClassHidGuardian, false);
                if (enumerateInfoList.Any())
                {
                    foreach (EnumerateInfo device in enumerateInfoList)
                    {
                        try
                        {
                            string DeviceInstanceId = ConvertPathToInstanceId(device.DevicePath);
                            DeviceRemove(GuidClassHidGuardian, DeviceInstanceId);
                        }
                        catch { }
                    }
                    TextBoxAppend(enumerateInfoList.Count + "x ghost HidGuardian found.");
                }
                else
                {
                    Debug.WriteLine("No ghost HidGuardian found.");
                    TextBoxAppend("No ghost HidGuardian found.");
                }
            }
            catch { }
        }

        void RemoveGhostXboxControllers()
        {
            try
            {
                List<EnumerateInfo> enumerateInfoList = EnumerateDevices(GuidClassX360Controller, false);
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
                    TextBoxAppend(enumerateInfoList.Count + "x ghost Xbox controller removed.");
                }
                else
                {
                    Debug.WriteLine("No ghost Xbox controllers found.");
                    TextBoxAppend("No ghost Xbox controllers found.");
                }
            }
            catch { }
        }
    }
}