namespace LibraryUsb
{
    public partial class VigemBusDevice : WinUsbDevice
    {
        //VigemBus v1.21.442.0
        public const int VirtualIdOffset = 20;

        public enum IoControlCodesVirtual : uint
        {
            VIGEM_PLUGIN = 0x2AA004,
            VIGEM_UNPLUG = 0x2AA008,
            VIGEM_INPUT = 0x2AA808,
            VIGEM_OUTPUT = 0x2AE804,
        }

        public enum ByteArraySizes : uint
        {
            Plugin = 16,
            Unplug = 8,
            Input = 20,
            Output = 12
        }

        public enum VIGEM_TARGET_TYPE : uint
        {
            Xbox360Wired = 0,
            XboxOneWired = 1,
            DualShock4Wired = 2
        }
    }
}