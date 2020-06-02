namespace LibraryShared
{
    public partial class Classes
    {
        public class KeypadStatus
        {
            //Raw Thumbs
            public bool ThumbLeftUp = false;
            public bool ThumbLeftDown = false;
            public bool ThumbLeftLeft = false;
            public bool ThumbLeftRight = false;
            public bool ThumbRightUp = false;
            public bool ThumbRightDown = false;
            public bool ThumbRightLeft = false;
            public bool ThumbRightRight = false;

            //Raw Triggers
            public bool TriggerLeft = false;
            public bool TriggerRight = false;

            //Raw D-Pad
            public bool DPadUp = false;
            public bool DPadDown = false;
            public bool DPadLeft = false;
            public bool DPadRight = false;

            //Raw Buttons
            public bool ButtonA = false;
            public bool ButtonB = false;
            public bool ButtonX = false;
            public bool ButtonY = false;

            public bool ButtonBack = false;
            public bool ButtonStart = false;
            public bool ButtonGuide = false;

            public bool ButtonShoulderLeft = false;
            public bool ButtonShoulderRight = false;
            public bool ButtonThumbLeft = false;
            public bool ButtonThumbRight = false;
            public bool ButtonTriggerLeft = false;
            public bool ButtonTriggerRight = false;
        }
    }
}