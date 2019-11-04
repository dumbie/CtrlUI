using System;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropCom;

namespace IMMDevice
{
    public enum STGM
    {
        STGM_READ = 0,
        STGM_WRITE = 1,
        STGM_READWRITE = 2
    }

    public enum DeviceState : uint
    {
        ACTIVE = 0x00000001,
        DISABLED = 0x00000002,
        NOTPRESENT = 0x00000004,
        UNPLUGGED = 0x00000008,
        MASK_ALL = 0x0000000F
    }

    public enum EDataFlow
    {
        eRender = 0,
        eCapture = 1,
        eAll = 2,
        EDataFlow_enum_count = 3
    }

    public enum ERole
    {
        eConsole = 0,
        eMultimedia = 1,
        eCommunications = 2,
        ERole_enum_count = 3
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDevice
    {
        void Activate(ref Guid id, uint dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.Interface)] out object ppInterface);
        [return: MarshalAs(UnmanagedType.Interface)]
        IPropertyStore OpenPropertyStore(STGM stgmAccess);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetId();
        DeviceState GetState();
    }

    [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    public class MMDeviceEnumerator { }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceEnumerator
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDeviceCollection EnumAudioEndpoints(EDataFlow dataFlow, DeviceState dwStateMask);
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDevice GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role);
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDevice GetDevice([MarshalAs(UnmanagedType.LPWStr)]string pwstrId);
    }

    [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceCollection
    {
        uint GetCount();
        [return: MarshalAs(UnmanagedType.Interface)]
        IMMDevice Item(uint nDevice);
    }
}