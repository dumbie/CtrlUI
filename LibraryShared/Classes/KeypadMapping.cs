using System.ComponentModel;
using System.Runtime.CompilerServices;
using static LibraryUsb.VirtualHidDevice;

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
            public KeysDDCode? ThumbLeftUpMod { get; set; } = null;
            public KeysDDCode? ThumbLeftUp { get; set; } = null;
            public KeysDDCode? ThumbLeftDownMod { get; set; } = null;
            public KeysDDCode? ThumbLeftDown { get; set; } = null;
            public KeysDDCode? ThumbLeftLeftMod { get; set; } = null;
            public KeysDDCode? ThumbLeftLeft { get; set; } = null;
            public KeysDDCode? ThumbLeftRightMod { get; set; } = null;
            public KeysDDCode? ThumbLeftRight { get; set; } = null;
            public KeysDDCode? ThumbRightUpMod { get; set; } = null;
            public KeysDDCode? ThumbRightUp { get; set; } = null;
            public KeysDDCode? ThumbRightDownMod { get; set; } = null;
            public KeysDDCode? ThumbRightDown { get; set; } = null;
            public KeysDDCode? ThumbRightLeftMod { get; set; } = null;
            public KeysDDCode? ThumbRightLeft { get; set; } = null;
            public KeysDDCode? ThumbRightRightMod { get; set; } = null;
            public KeysDDCode? ThumbRightRight { get; set; } = null;

            //Raw D-Pad
            public KeysDDCode? DPadUpMod { get; set; } = null;
            public KeysDDCode? DPadUp { get; set; } = null;
            public KeysDDCode? DPadDownMod { get; set; } = null;
            public KeysDDCode? DPadDown { get; set; } = null;
            public KeysDDCode? DPadLeftMod { get; set; } = null;
            public KeysDDCode? DPadLeft { get; set; } = null;
            public KeysDDCode? DPadRightMod { get; set; } = null;
            public KeysDDCode? DPadRight { get; set; } = null;

            //Raw Buttons
            public KeysDDCode? ButtonAMod { get; set; } = null;
            public KeysDDCode? ButtonA { get; set; } = null;
            public KeysDDCode? ButtonBMod { get; set; } = null;
            public KeysDDCode? ButtonB { get; set; } = null;
            public KeysDDCode? ButtonXMod { get; set; } = null;
            public KeysDDCode? ButtonX { get; set; } = null;
            public KeysDDCode? ButtonYMod { get; set; } = null;
            public KeysDDCode? ButtonY { get; set; } = null;

            public KeysDDCode? ButtonBackMod { get; set; } = null;
            public KeysDDCode? ButtonBack { get; set; } = null;
            public KeysDDCode? ButtonStartMod { get; set; } = null;
            public KeysDDCode? ButtonStart { get; set; } = null;
            public KeysDDCode? ButtonGuideMod { get; set; } = null;
            public KeysDDCode? ButtonGuide { get; set; } = null;

            public KeysDDCode? ButtonShoulderLeftMod { get; set; } = null;
            public KeysDDCode? ButtonShoulderLeft { get; set; } = null;
            public KeysDDCode? ButtonShoulderRightMod { get; set; } = null;
            public KeysDDCode? ButtonShoulderRight { get; set; } = null;
            public KeysDDCode? ButtonThumbLeftMod { get; set; } = null;
            public KeysDDCode? ButtonThumbLeft { get; set; } = null;
            public KeysDDCode? ButtonThumbRightMod { get; set; } = null;
            public KeysDDCode? ButtonThumbRight { get; set; } = null;
            public KeysDDCode? ButtonTriggerLeftMod { get; set; } = null;
            public KeysDDCode? ButtonTriggerLeft { get; set; } = null;
            public KeysDDCode? ButtonTriggerRightMod { get; set; } = null;
            public KeysDDCode? ButtonTriggerRight { get; set; } = null;

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}