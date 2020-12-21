namespace DirectXInput
{
    partial class WindowMain
    {
        public enum DsuMessageType : uint
        {
            DSUC_VersionReq = 0x100000,
            DSUS_VersionRsp = 0x100000,
            DSUC_ListPorts = 0x100001,
            DSUS_PortInfo = 0x100001,
            DSUC_PadDataReq = 0x100002,
            DSUS_PadDataRsp = 0x100002
        }

        public enum DsuState : byte
        {
            Disconnected = 0x00,
            Reserved = 0x01,
            Connected = 0x02
        }

        public enum DsuConnectionType : byte
        {
            None = 0x00,
            Usb = 0x01,
            Bluetooth = 0x02
        }

        public enum DsuModel : byte
        {
            None = 0,
            DualShock3 = 1,
            DualShock4 = 2,
            Generic = 3
        }

        public enum DsuBattery : byte
        {
            None = 0x00,
            Dying = 0x01,
            Low = 0x02,
            Medium = 0x03,
            High = 0x04,
            Full = 0x05,
            Charging = 0xEE,
            Charged = 0xEF
        }
    }
}