namespace LibraryShared
{
    public partial class Classes
    {
        public class KeypadStatus
        {
            //Raw Thumbs
            public KeypadDownStatus ThumbLeftUp = new KeypadDownStatus();
            public KeypadDownStatus ThumbLeftDown = new KeypadDownStatus();
            public KeypadDownStatus ThumbLeftLeft = new KeypadDownStatus();
            public KeypadDownStatus ThumbLeftRight = new KeypadDownStatus();
            public KeypadDownStatus ThumbRightUp = new KeypadDownStatus();
            public KeypadDownStatus ThumbRightDown = new KeypadDownStatus();
            public KeypadDownStatus ThumbRightLeft = new KeypadDownStatus();
            public KeypadDownStatus ThumbRightRight = new KeypadDownStatus();

            //Raw Triggers
            public KeypadDownStatus TriggerLeft = new KeypadDownStatus();
            public KeypadDownStatus TriggerRight = new KeypadDownStatus();

            //Raw D-Pad
            public KeypadDownStatus DPadUp = new KeypadDownStatus();
            public KeypadDownStatus DPadDown = new KeypadDownStatus();
            public KeypadDownStatus DPadLeft = new KeypadDownStatus();
            public KeypadDownStatus DPadRight = new KeypadDownStatus();

            //Raw Buttons
            public KeypadDownStatus ButtonA = new KeypadDownStatus();
            public KeypadDownStatus ButtonB = new KeypadDownStatus();
            public KeypadDownStatus ButtonX = new KeypadDownStatus();
            public KeypadDownStatus ButtonY = new KeypadDownStatus();

            public KeypadDownStatus ButtonBack = new KeypadDownStatus();
            public KeypadDownStatus ButtonStart = new KeypadDownStatus();
            public KeypadDownStatus ButtonGuide = new KeypadDownStatus();

            public KeypadDownStatus ButtonShoulderLeft = new KeypadDownStatus();
            public KeypadDownStatus ButtonShoulderRight = new KeypadDownStatus();
            public KeypadDownStatus ButtonThumbLeft = new KeypadDownStatus();
            public KeypadDownStatus ButtonThumbRight = new KeypadDownStatus();
            public KeypadDownStatus ButtonTriggerLeft = new KeypadDownStatus();
            public KeypadDownStatus ButtonTriggerRight = new KeypadDownStatus();
        }

        public class KeypadDownStatus
        {
            public bool Pressed { get; set; } = false;
            public int LastPress { get; set; } = 0;
        }
    }
}