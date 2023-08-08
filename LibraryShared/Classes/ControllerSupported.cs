namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerSupported
        {
            public string CodeName { get; set; } = "Unsupported";
            public string[] ProductIDs { get; set; }
            public string VendorID { get; set; }
            public bool HasTouchpad { get; set; }
            public bool HasGyroscope { get; set; }
            public bool HasAccelerometer { get; set; }
            public bool HasRumbleMode { get; set; }
            public bool HasRumbleTrigger { get; set; }
            public int OffsetWired { get; set; }
            public int OffsetWireless { get; set; }
            public ClassOffsetHeader OffsetHeader { get; set; } = new ClassOffsetHeader();
            public ClassOffsetButton OffsetButton { get; set; } = new ClassOffsetButton();

            public class ClassOffsetHeader
            {
                public int? ThumbLeftX { get; set; }
                public int? ThumbLeftY { get; set; }
                public int? ThumbRightX { get; set; }
                public int? ThumbRightY { get; set; }
                public int? ButtonsGroup1 { get; set; }
                public int? ButtonsGroup2 { get; set; }
                public int? ButtonsGroup3 { get; set; }
                public int? TriggerLeft { get; set; }
                public int? TriggerRight { get; set; }
                public int? Touchpad { get; set; }
                public int? Gyroscope { get; set; }
                public int? Accelerometer { get; set; }
                public int? BatteryLevel { get; set; }
                public int? BatteryStatus { get; set; }
                public int? Checksum { get; set; }
            }

            public class ClassOffsetButton
            {
                public ClassButtonDetails A { get; set; }
                public ClassButtonDetails B { get; set; }
                public ClassButtonDetails X { get; set; }
                public ClassButtonDetails Y { get; set; }
                public bool DPadDirect { get; set; }
                public ClassButtonDetails DPadLeft { get; set; }
                public ClassButtonDetails DPadRight { get; set; }
                public ClassButtonDetails DPadUp { get; set; }
                public ClassButtonDetails DPadDown { get; set; }
                public ClassButtonDetails ShoulderLeft { get; set; }
                public ClassButtonDetails ShoulderRight { get; set; }
                public ClassButtonDetails TriggerLeft { get; set; }
                public ClassButtonDetails TriggerRight { get; set; }
                public ClassButtonDetails ThumbLeft { get; set; }
                public ClassButtonDetails ThumbRight { get; set; }
                public ClassButtonDetails Start { get; set; }
                public ClassButtonDetails Back { get; set; }
                public ClassButtonDetails Touchpad { get; set; }
                public ClassButtonDetails Guide { get; set; }
                public ClassButtonDetails Media { get; set; }
            }

            public class ClassButtonDetails
            {
                public int Group { get; set; }
                public int Offset { get; set; }
            }
        }
    }
}