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
            public int ButtonDelayFirstMs { get; set; } = 400;
            public int ButtonDelayRepeatMs { get; set; } = 30;
            public double KeypadOpacity { get; set; } = 0.70;
            public int KeypadDisplayStyle { get; set; } = 0;
            public int KeypadDisplaySize { get; set; } = 60;
            public bool KeypadMouseMoveEnabled { get; set; } = false;
            public double KeypadMouseMoveSensitivity { get; set; } = 7.50;

            //Raw Thumbs
            public KeysVirtual? ThumbLeftUpMod { get; set; } = null;
            public KeysVirtual? ThumbLeftUp { get; set; } = null;
            public KeysVirtual? ThumbLeftDownMod { get; set; } = null;
            public KeysVirtual? ThumbLeftDown { get; set; } = null;
            public KeysVirtual? ThumbLeftLeftMod { get; set; } = null;
            public KeysVirtual? ThumbLeftLeft { get; set; } = null;
            public KeysVirtual? ThumbLeftRightMod { get; set; } = null;
            public KeysVirtual? ThumbLeftRight { get; set; } = null;
            public KeysVirtual? ThumbRightUpMod { get; set; } = null;
            public KeysVirtual? ThumbRightUp { get; set; } = null;
            public KeysVirtual? ThumbRightDownMod { get; set; } = null;
            public KeysVirtual? ThumbRightDown { get; set; } = null;
            public KeysVirtual? ThumbRightLeftMod { get; set; } = null;
            public KeysVirtual? ThumbRightLeft { get; set; } = null;
            public KeysVirtual? ThumbRightRightMod { get; set; } = null;
            public KeysVirtual? ThumbRightRight { get; set; } = null;

            //Raw D-Pad
            public KeysVirtual? DPadUpMod { get; set; } = null;
            public KeysVirtual? DPadUp { get; set; } = null;
            public KeysVirtual? DPadDownMod { get; set; } = null;
            public KeysVirtual? DPadDown { get; set; } = null;
            public KeysVirtual? DPadLeftMod { get; set; } = null;
            public KeysVirtual? DPadLeft { get; set; } = null;
            public KeysVirtual? DPadRightMod { get; set; } = null;
            public KeysVirtual? DPadRight { get; set; } = null;

            //Raw Buttons
            public KeysVirtual? ButtonAMod { get; set; } = null;
            public KeysVirtual? ButtonA { get; set; } = null;
            public KeysVirtual? ButtonBMod { get; set; } = null;
            public KeysVirtual? ButtonB { get; set; } = null;
            public KeysVirtual? ButtonXMod { get; set; } = null;
            public KeysVirtual? ButtonX { get; set; } = null;
            public KeysVirtual? ButtonYMod { get; set; } = null;
            public KeysVirtual? ButtonY { get; set; } = null;

            public KeysVirtual? ButtonBackMod { get; set; } = null;
            public KeysVirtual? ButtonBack { get; set; } = null;
            public KeysVirtual? ButtonStartMod { get; set; } = null;
            public KeysVirtual? ButtonStart { get; set; } = null;
            public KeysVirtual? ButtonGuideMod { get; set; } = null;
            public KeysVirtual? ButtonGuide { get; set; } = null;

            public KeysVirtual? ButtonShoulderLeftMod { get; set; } = null;
            public KeysVirtual? ButtonShoulderLeft { get; set; } = null;
            public KeysVirtual? ButtonShoulderRightMod { get; set; } = null;
            public KeysVirtual? ButtonShoulderRight { get; set; } = null;
            public KeysVirtual? ButtonThumbLeftMod { get; set; } = null;
            public KeysVirtual? ButtonThumbLeft { get; set; } = null;
            public KeysVirtual? ButtonThumbRightMod { get; set; } = null;
            public KeysVirtual? ButtonThumbRight { get; set; } = null;
            public KeysVirtual? ButtonTriggerLeftMod { get; set; } = null;
            public KeysVirtual? ButtonTriggerLeft { get; set; } = null;
            public KeysVirtual? ButtonTriggerRightMod { get; set; } = null;
            public KeysVirtual? ButtonTriggerRight { get; set; } = null;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}