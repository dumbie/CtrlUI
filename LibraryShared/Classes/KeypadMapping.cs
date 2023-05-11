using System.ComponentModel;
using System.Runtime.CompilerServices;
using static ArnoldVinkCode.AVInputOutputClass;

namespace LibraryShared
{
    public partial class Classes
    {
        public class KeypadMapping : INotifyPropertyChanged
        {
            //Mapping Details
            public string Name { get; set; } = string.Empty;
            public double KeypadOpacity { get; set; } = 0.70;
            public int KeypadDisplayStyle { get; set; } = 0;
            public int KeypadDisplaySize { get; set; } = 60;
            public bool KeypadMouseMoveEnabled { get; set; } = false;
            public double KeypadMouseMoveSensitivity { get; set; } = 7.50;

            //Raw Thumbs
            public KeysModifierHid ThumbLeftUpMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbLeftUpMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbLeftUp { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbLeftDownMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbLeftDownMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbLeftDown { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbLeftLeftMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbLeftLeftMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbLeftLeft { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbLeftRightMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbLeftRightMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbLeftRight { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbRightUpMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbRightUpMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbRightUp { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbRightDownMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbRightDownMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbRightDown { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbRightLeftMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbRightLeftMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbRightLeft { get; set; } = KeysHid.None;
            public KeysModifierHid ThumbRightRightMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ThumbRightRightMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ThumbRightRight { get; set; } = KeysHid.None;

            //Raw DPad
            public KeysModifierHid DPadUpMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid DPadUpMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid DPadUp { get; set; } = KeysHid.None;
            public KeysModifierHid DPadDownMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid DPadDownMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid DPadDown { get; set; } = KeysHid.None;
            public KeysModifierHid DPadLeftMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid DPadLeftMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid DPadLeft { get; set; } = KeysHid.None;
            public KeysModifierHid DPadRightMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid DPadRightMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid DPadRight { get; set; } = KeysHid.None;

            //Raw Buttons
            public KeysModifierHid ButtonAMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonAMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonA { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonBMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonBMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonB { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonXMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonXMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonX { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonYMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonYMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonY { get; set; } = KeysHid.None;

            public KeysModifierHid ButtonBackMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonBackMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonBack { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonStartMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonStartMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonStart { get; set; } = KeysHid.None;

            public KeysModifierHid ButtonShoulderLeftMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonShoulderLeftMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonShoulderLeft { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonShoulderRightMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonShoulderRightMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonShoulderRight { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonThumbLeftMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonThumbLeftMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonThumbLeft { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonThumbRightMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonThumbRightMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonThumbRight { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonTriggerLeftMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonTriggerLeftMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonTriggerLeft { get; set; } = KeysHid.None;
            public KeysModifierHid ButtonTriggerRightMod0 { get; set; } = KeysModifierHid.None;
            public KeysModifierHid ButtonTriggerRightMod1 { get; set; } = KeysModifierHid.None;
            public KeysHid ButtonTriggerRight { get; set; } = KeysHid.None;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}