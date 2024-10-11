namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerSupported
        {
            public string CodeName { get; set; } = "Unsupported";
            public string[] ProductIDs { get; set; }
            public string VendorID { get; set; }
            public bool HasInputOnDemand { get; set; }
            public bool HasRumbleMode { get; set; }
            public bool HasRumbleTrigger { get; set; }
            public bool HasLedStatus { get; set; }
            public bool HasLedPlayer { get; set; }
            public bool HasLedMedia { get; set; }
            public int OffsetWired { get; set; }
            public int OffsetWireless { get; set; }
            public ClassOffsetHeader OffsetHeader { get; set; } = new ClassOffsetHeader();
            public ClassOffsetDPad OffsetDPad { get; set; } = new ClassOffsetDPad();
            public ClassOffsetButton OffsetButton { get; set; } = new ClassOffsetButton();

            public class ClassOffsetHeader
            {
                public int? DPadLeft { get; set; }
                public int? DPadRight { get; set; }
                public int? ThumbLeftX { get; set; }
                public int? ThumbLeftY { get; set; }
                public int? ThumbLeftZ { get; set; }
                public int? ThumbRightX { get; set; }
                public int? ThumbRightY { get; set; }
                public int? ThumbRightZ { get; set; }
                public int? TriggerLeft { get; set; }
                public int? TriggerRight { get; set; }
                public int? Touchpad { get; set; }
                public int? Gyroscope { get; set; }
                public int? Accelerometer { get; set; }
                public int? BatteryLevel { get; set; }
                public int? BatteryStatus { get; set; }
                public int? Checksum { get; set; }
            }

            public class ClassOffsetDPad
            {
                public int? DPadN { get; set; }
                public int? DPadNE { get; set; }
                public int? DPadE { get; set; }
                public int? DPadSE { get; set; }
                public int? DPadS { get; set; }
                public int? DPadSW { get; set; }
                public int? DPadW { get; set; }
                public int? DPadNW { get; set; }
            }

            public class ClassOffsetButton
            {
                public ClassButtonDetails A { get; set; }
                public ClassButtonDetails B { get; set; }
                public ClassButtonDetails X { get; set; }
                public ClassButtonDetails Y { get; set; }
                public ClassButtonDetails ShoulderLeft { get; set; }
                public ClassButtonDetails ShoulderRight { get; set; }
                public ClassButtonDetails TriggerLeft { get; set; }
                public ClassButtonDetails TriggerRight { get; set; }
                public ClassButtonDetails ThumbLeft { get; set; }
                public ClassButtonDetails ThumbRight { get; set; }
                public ClassButtonDetails Back { get; set; }
                public ClassButtonDetails Start { get; set; }
                public ClassButtonDetails Guide { get; set; }
                public ClassButtonDetails One { get; set; }
                public ClassButtonDetails Two { get; set; }
                public ClassButtonDetails Three { get; set; }
                public ClassButtonDetails Four { get; set; }
                public ClassButtonDetails Five { get; set; }
                public ClassButtonDetails Six { get; set; }
            }

            public class ClassButtonDetails
            {
                public int? Header { get; set; }
                public int? Group { get; set; }
                public int? Offset { get; set; }
            }
        }
    }
}