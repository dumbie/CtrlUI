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
            public KeyboardModifiers ThumbLeftUpMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbLeftUpMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbLeftUp { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbLeftDownMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbLeftDownMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbLeftDown { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbLeftLeftMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbLeftLeftMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbLeftLeft { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbLeftRightMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbLeftRightMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbLeftRight { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbRightUpMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbRightUpMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbRightUp { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbRightDownMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbRightDownMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbRightDown { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbRightLeftMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbRightLeftMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbRightLeft { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ThumbRightRightMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ThumbRightRightMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ThumbRightRight { get; set; } = KeyboardKeys.None;

            //Raw D-Pad
            public KeyboardModifiers DPadUpMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers DPadUpMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys DPadUp { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers DPadDownMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers DPadDownMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys DPadDown { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers DPadLeftMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers DPadLeftMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys DPadLeft { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers DPadRightMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers DPadRightMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys DPadRight { get; set; } = KeyboardKeys.None;

            //Raw Buttons
            public KeyboardModifiers ButtonAMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonAMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonA { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonBMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonBMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonB { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonXMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonXMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonX { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonYMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonYMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonY { get; set; } = KeyboardKeys.None;

            public KeyboardModifiers ButtonBackMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonBackMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonBack { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonStartMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonStartMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonStart { get; set; } = KeyboardKeys.None;

            public KeyboardModifiers ButtonShoulderLeftMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonShoulderLeftMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonShoulderLeft { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonShoulderRightMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonShoulderRightMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonShoulderRight { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonThumbLeftMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonThumbLeftMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonThumbLeft { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonThumbRightMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonThumbRightMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonThumbRight { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonTriggerLeftMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonTriggerLeftMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonTriggerLeft { get; set; } = KeyboardKeys.None;
            public KeyboardModifiers ButtonTriggerRightMod0 { get; set; } = KeyboardModifiers.None;
            public KeyboardModifiers ButtonTriggerRightMod1 { get; set; } = KeyboardModifiers.None;
            public KeyboardKeys ButtonTriggerRight { get; set; } = KeyboardKeys.None;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}