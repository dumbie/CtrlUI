using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropCom;

namespace IMMDevice
{
    public class PolicyConfigClient
    {
        IPolicyConfig policyConfig = (IPolicyConfig)new PolicyConfig();

        public void SetEndpointVisibility(string deviceId, bool isVisible)
        {
            try
            {
                policyConfig.SetEndpointVisibility(deviceId, isVisible ? (short)1 : (short)0);
            }
            catch { }
        }

        public void SetDefaultEndpoint(string deviceId, ERole role)
        {
            try
            {
                policyConfig.SetDefaultEndpoint(deviceId, role);
            }
            catch { }
        }
    }

    [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
    public class PolicyConfig { }

    [Guid("F8679F50-850A-41CF-9C72-430F290290C8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPolicyConfig
    {
        void GetMixFormat();
        void GetDeviceFormat();
        void ResetDeviceFormat();
        void SetDeviceFormat();
        void GetProcessingPeriod();
        void SetProcessingPeriod();
        void GetShareMode();
        void SetShareMode();
        void GetPropertyValue([MarshalAs(UnmanagedType.LPWStr)]string wszDeviceId, ref PropertyKey pKey, ref PropertyVariant pVar);
        void SetPropertyValue([MarshalAs(UnmanagedType.LPWStr)]string wszDeviceId, ref PropertyKey pKey, ref PropertyVariant pVar);
        void SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)]string wszDeviceId, ERole eRole);
        void SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr)]string wszDeviceId, [MarshalAs(UnmanagedType.I2)]short isVisible);
    }
}