using System.ComponentModel;
using System.Runtime.CompilerServices;
using static LibraryUsb.FakerInputDevice;

namespace LibraryShared
{
    public partial class Classes
    {
        public class KeypadMapping : INotifyPropertyChanged
        {
            //Mapping Details
            public string Name { get; set; } = string.Empty;
            public int ButtonDelayFirstMs { get; set; } = 400;
            public int ButtonDelayRepeatMs { get; set; } = 30;
            public double KeypadOpacity { get; set; } = 0.70;
            public int KeypadDisplayStyle { get; set; } = 0;
            public int KeypadDisplaySize { get; set; } = 60;
            public bool KeypadMouseMoveEnabled { get; set; } = false;
            public double KeypadMouseMoveSensitivity { get; set; } = 7.50;

            //Raw Thumbs
            public KeyboardModifiers? ThumbLeftUpMod { get; set; } = null;
            public KeyboardKeys? ThumbLeftUp { get; set; } = null;
            public KeyboardModifiers? ThumbLeftDownMod { get; set; } = null;
            public KeyboardKeys? ThumbLeftDown { get; set; } = null;
            public KeyboardModifiers? ThumbLeftLeftMod { get; set; } = null;
            public KeyboardKeys? ThumbLeftLeft { get; set; } = null;
            public KeyboardModifiers? ThumbLeftRightMod { get; set; } = null;
            public KeyboardKeys? ThumbLeftRight { get; set; } = null;
            public KeyboardModifiers? ThumbRightUpMod { get; set; } = null;
            public KeyboardKeys? ThumbRightUp { get; set; } = null;
            public KeyboardModifiers? ThumbRightDownMod { get; set; } = null;
            public KeyboardKeys? ThumbRightDown { get; set; } = null;
            public KeyboardModifiers? ThumbRightLeftMod { get; set; } = null;
            public KeyboardKeys? ThumbRightLeft { get; set; } = null;
            public KeyboardModifiers? ThumbRightRightMod { get; set; } = null;
            public KeyboardKeys? ThumbRightRight { get; set; } = null;

            //Raw D-Pad
            public KeyboardModifiers? DPadUpMod { get; set; } = null;
            public KeyboardKeys? DPadUp { get; set; } = null;
            public KeyboardModifiers? DPadDownMod { get; set; } = null;
            public KeyboardKeys? DPadDown { get; set; } = null;
            public KeyboardModifiers? DPadLeftMod { get; set; } = null;
            public KeyboardKeys? DPadLeft { get; set; } = null;
            public KeyboardModifiers? DPadRightMod { get; set; } = null;
            public KeyboardKeys? DPadRight { get; set; } = null;

            //Raw Buttons
            public KeyboardModifiers? ButtonAMod { get; set; } = null;
            public KeyboardKeys? ButtonA { get; set; } = null;
            public KeyboardModifiers? ButtonBMod { get; set; } = null;
            public KeyboardKeys? ButtonB { get; set; } = null;
            public KeyboardModifiers? ButtonXMod { get; set; } = null;
            public KeyboardKeys? ButtonX { get; set; } = null;
            public KeyboardModifiers? ButtonYMod { get; set; } = null;
            public KeyboardKeys? ButtonY { get; set; } = null;

            public KeyboardModifiers? ButtonBackMod { get; set; } = null;
            public KeyboardKeys? ButtonBack { get; set; } = null;
            public KeyboardModifiers? ButtonStartMod { get; set; } = null;
            public KeyboardKeys? ButtonStart { get; set; } = null;
            public KeyboardModifiers? ButtonGuideMod { get; set; } = null;
            public KeyboardKeys? ButtonGuide { get; set; } = null;

            public KeyboardModifiers? ButtonShoulderLeftMod { get; set; } = null;
            public KeyboardKeys? ButtonShoulderLeft { get; set; } = null;
            public KeyboardModifiers? ButtonShoulderRightMod { get; set; } = null;
            public KeyboardKeys? ButtonShoulderRight { get; set; } = null;
            public KeyboardModifiers? ButtonThumbLeftMod { get; set; } = null;
            public KeyboardKeys? ButtonThumbLeft { get; set; } = null;
            public KeyboardModifiers? ButtonThumbRightMod { get; set; } = null;
            public KeyboardKeys? ButtonThumbRight { get; set; } = null;
            public KeyboardModifiers? ButtonTriggerLeftMod { get; set; } = null;
            public KeyboardKeys? ButtonTriggerLeft { get; set; } = null;
            public KeyboardModifiers? ButtonTriggerRightMod { get; set; } = null;
            public KeyboardKeys? ButtonTriggerRight { get; set; } = null;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}