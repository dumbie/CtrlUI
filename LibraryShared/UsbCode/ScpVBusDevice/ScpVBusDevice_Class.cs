namespace LibraryUsb
{
    public partial class ScpVBusDevice : WinUsbDevice
    {
        //ScpVBus v1.7.1.1
        public enum IoControlCodesVirtual : uint
        {
            SCP_PLUGIN = 0x2A4000,
            SCP_UNPLUG = 0x2A4004,
            SCP_REPORT = 0x2A400C
        }

        ////ScpVBus v1.7.1.2
        //public enum IoControlCodesVirtual : uint
        //{
        //    SCP_PLUGIN = 0x2AA004,
        //    SCP_UNPLUG = 0x2AA008,
        //    SCP_REPORT = 0x2AE010
        //}

        public enum ByteArraySizes : int
        {
            Plugin = 16,
            Unplug = 16,
            Input = 28,
            Output = 10
        }
    }
}