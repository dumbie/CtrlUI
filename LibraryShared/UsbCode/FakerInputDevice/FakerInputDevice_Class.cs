using System;
using System.Runtime.InteropServices;

namespace LibraryUsb
{
    public partial class FakerInputDevice
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_CONTROL_REPORT_HEADER
        {
            public byte ReportID;
            public byte ReportLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_KEYBOARD_REPORT
        {
            public byte ReportID;
            public byte ModifierCodes;
            public byte Reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = KBD_KEY_CODES)]
            public byte[] KeyCodes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_MULTIMEDIA_REPORT
        {
            public byte ReportID;
            public byte MultimediaKey0;
            public byte MultimediaKey1;
            public byte MultimediaKey2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_RELATIVE_MOUSE_REPORT
        {
            public byte ReportID;
            public byte Button;
            public short XValue;
            public short YValue;
            public byte VWheelPosition;
            public byte HWheelPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FAKERINPUT_ABSOLUTE_MOUSE_REPORT
        {
            public byte ReportID;
            public byte Button;
            public ushort XValue;
            public ushort YValue;
            public byte VWheelPosition;
            public byte HWheelPosition;
        }

        public enum FAKERINPUT_REPORT_ID : byte
        {
            REPORTID_KEYBOARD = 0x01,
            REPORTID_MULTIMEDIA = 0x02,
            REPORTID_RELATIVE_MOUSE = 0x03,
            REPORTID_ABSOLUTE_MOUSE = 0x04,
            REPORTID_CONTROL = 0x40
        }

        public enum MouseButtons : byte
        {
            None = 0x00,
            LeftButton = 0x01,
            RightButton = 0x02,
            MiddleButton = 0x04,
            XButton1 = 0x08,
            XButton2 = 0x10
        }

        public enum KeyboardMultimedia : byte
        {
            None = 0,
            Next = 1,
            Previous = 2,
            Stop = 4,
            PlayPause = 8,
            VolumeMute = 16,
            VolumeDown = 32,
            VolumeUp = 64
        }

        public enum KeyboardModifiers : byte
        {
            None = 0,
            ControlLeft = 1,
            ShiftLeft = 2,
            AltLeft = 4,
            WindowsLeft = 8,
            ControlRight = 16,
            ShiftRight = 32,
            AltRight = 64,
            WindowsRight = 128
        }

        public enum KeyboardKeys : byte
        {
            None = 0,
            A = 4,
            B = 5,
            C = 6,
            D = 7,
            E = 8,
            F = 9,
            G = 10,
            H = 11,
            I = 12,
            J = 13,
            K = 14,
            L = 15,
            M = 16,
            N = 17,
            O = 18,
            P = 19,
            Q = 20,
            R = 21,
            S = 22,
            T = 23,
            U = 24,
            V = 25,
            W = 26,
            X = 27,
            Y = 28,
            Z = 29,
            Digit1 = 30,
            Digit2 = 31,
            Digit3 = 32,
            Digit4 = 33,
            Digit5 = 34,
            Digit6 = 35,
            Digit7 = 36,
            Digit8 = 37,
            Digit9 = 38,
            Digit0 = 39,
            Enter = 40,
            Escape = 41,
            Backspace = 42,
            Tab = 43,
            Space = 44,
            Minus = 45,
            Equal = 46,
            LeftBracket = 47,
            RightBracket = 48,
            Backslash = 49,
            Semicolon = 51,
            Quote = 52,
            Backtick = 53,
            Comma = 54,
            Period = 55,
            Slash = 56,
            CapsLock = 57,
            F1 = 58,
            F2 = 59,
            F3 = 60,
            F4 = 61,
            F5 = 62,
            F6 = 63,
            F7 = 64,
            F8 = 65,
            F9 = 66,
            F10 = 67,
            F11 = 68,
            F12 = 69,
            PrintScreen = 70,
            ScrollLock = 71,
            Pause = 72,
            Insert = 73,
            Home = 74,
            PageUp = 75,
            Delete = 76,
            End = 77,
            PageDown = 78,
            ArrowRight = 79,
            ArrowLeft = 80,
            ArrowDown = 81,
            ArrowUp = 82,
            NumLock = 83,
            NumpadSlash = 84,
            NumpadAsterisk = 85,
            NumpadMinus = 86,
            NumpadPlus = 87,
            NumpadEnter = 88,
            Numpad1 = 89,
            Numpad2 = 90,
            Numpad3 = 91,
            Numpad4 = 92,
            Numpad5 = 93,
            Numpad6 = 94,
            Numpad7 = 95,
            Numpad8 = 96,
            Numpad9 = 97,
            Numpad0 = 98,
            NumpadPeriod = 99,
            Menu = 101,
        }

        public string GetMouseButtonsName(MouseButtons mouseButton, bool shortName)
        {
            try
            {
                switch (mouseButton)
                {
                    case MouseButtons.LeftButton:
                        if (shortName) { return "Left"; } else { return "Left Button"; }
                    case MouseButtons.RightButton:
                        if (shortName) { return "Right"; } else { return "Right Button"; }
                    case MouseButtons.MiddleButton:
                        if (shortName) { return "Middle"; } else { return "Middle Button"; }
                    case MouseButtons.XButton1:
                        if (shortName) { return "Side1"; } else { return "Side Button 1"; }
                    case MouseButtons.XButton2:
                        if (shortName) { return "Side2"; } else { return "Side Button 2"; }
                }
            }
            catch { }
            return Enum.GetName(typeof(MouseButtons), mouseButton);
        }

        public string GetKeyboardMultimediaName(KeyboardMultimedia multimediaKey, bool shortName)
        {
            try
            {
                switch (multimediaKey)
                {
                    case KeyboardMultimedia.Next:
                        if (shortName) { return "Next"; } else { return "Media Next"; }
                    case KeyboardMultimedia.Previous:
                        if (shortName) { return "Prev"; } else { return "Media Previous"; }
                    case KeyboardMultimedia.Stop:
                        if (shortName) { return "Stop"; } else { return "Media Stop"; }
                    case KeyboardMultimedia.PlayPause:
                        if (shortName) { return "Play"; } else { return "Media Play/Pause"; }
                    case KeyboardMultimedia.VolumeMute:
                        if (shortName) { return "Mute"; } else { return "Volume Mute"; }
                    case KeyboardMultimedia.VolumeDown:
                        if (shortName) { return "VolDn"; } else { return "Volume Down"; }
                    case KeyboardMultimedia.VolumeUp:
                        if (shortName) { return "VolUp"; } else { return "Volume Up"; }
                }
            }
            catch { }
            return Enum.GetName(typeof(KeyboardMultimedia), multimediaKey);
        }

        public string GetKeyboardModifiersName(KeyboardModifiers keyboardModifier, bool shortName)
        {
            try
            {
                switch (keyboardModifier)
                {
                    case KeyboardModifiers.ControlLeft:
                    case KeyboardModifiers.ControlRight:
                        if (shortName) { return "Ctrl"; } else { return "Control"; }
                    case KeyboardModifiers.ShiftLeft:
                    case KeyboardModifiers.ShiftRight:
                        return "Shift";
                    case KeyboardModifiers.AltLeft:
                    case KeyboardModifiers.AltRight:
                        if (shortName) { return "Alt"; } else { return "Alternate"; }
                    case KeyboardModifiers.WindowsLeft:
                    case KeyboardModifiers.WindowsRight:
                        if (shortName) { return "Win"; } else { return "Windows"; }
                }
            }
            catch { }
            return Enum.GetName(typeof(KeyboardModifiers), keyboardModifier);
        }

        public string GetKeyboardKeysName(KeyboardKeys keyboardKey, bool shortName)
        {
            try
            {
                switch (keyboardKey)
                {
                    case KeyboardKeys.A:
                    case KeyboardKeys.B:
                    case KeyboardKeys.C:
                    case KeyboardKeys.D:
                    case KeyboardKeys.E:
                    case KeyboardKeys.F:
                    case KeyboardKeys.G:
                    case KeyboardKeys.H:
                    case KeyboardKeys.I:
                    case KeyboardKeys.J:
                    case KeyboardKeys.K:
                    case KeyboardKeys.L:
                    case KeyboardKeys.M:
                    case KeyboardKeys.N:
                    case KeyboardKeys.O:
                    case KeyboardKeys.P:
                    case KeyboardKeys.Q:
                    case KeyboardKeys.R:
                    case KeyboardKeys.S:
                    case KeyboardKeys.T:
                    case KeyboardKeys.U:
                    case KeyboardKeys.V:
                    case KeyboardKeys.W:
                    case KeyboardKeys.X:
                    case KeyboardKeys.Y:
                    case KeyboardKeys.Z:
                        return Enum.GetName(typeof(KeyboardKeys), keyboardKey);
                    case KeyboardKeys.Digit1:
                        return "1";
                    case KeyboardKeys.Digit2:
                        return "2";
                    case KeyboardKeys.Digit3:
                        return "3";
                    case KeyboardKeys.Digit4:
                        return "4";
                    case KeyboardKeys.Digit5:
                        return "5";
                    case KeyboardKeys.Digit6:
                        return "6";
                    case KeyboardKeys.Digit7:
                        return "7";
                    case KeyboardKeys.Digit8:
                        return "8";
                    case KeyboardKeys.Digit9:
                        return "9";
                    case KeyboardKeys.Digit0:
                        return "0";
                    case KeyboardKeys.Enter:
                        return "Enter";
                    case KeyboardKeys.Escape:
                        if (shortName) { return "Esc"; } else { return "Escape"; }
                    case KeyboardKeys.Backspace:
                        if (shortName) { return "BkSpc"; } else { return "Backspace"; }
                    case KeyboardKeys.Tab:
                        return "Tab";
                    case KeyboardKeys.Space:
                        if (shortName) { return "Spc"; } else { return "Spacebar"; }
                    case KeyboardKeys.Minus:
                        return "-";
                    case KeyboardKeys.Equal:
                        return "=";
                    case KeyboardKeys.LeftBracket:
                        return "[";
                    case KeyboardKeys.RightBracket:
                        return "]";
                    case KeyboardKeys.Backslash:
                        return "\\";
                    case KeyboardKeys.Semicolon:
                        return ";";
                    case KeyboardKeys.Quote:
                        return "'";
                    case KeyboardKeys.Backtick:
                        return "`";
                    case KeyboardKeys.Comma:
                        return ",";
                    case KeyboardKeys.Period:
                        return ".";
                    case KeyboardKeys.Slash:
                        return "/";
                    case KeyboardKeys.CapsLock:
                        if (shortName) { return "CpLck"; } else { return "Caps Lock"; }
                    case KeyboardKeys.F1:
                    case KeyboardKeys.F2:
                    case KeyboardKeys.F3:
                    case KeyboardKeys.F4:
                    case KeyboardKeys.F5:
                    case KeyboardKeys.F6:
                    case KeyboardKeys.F7:
                    case KeyboardKeys.F8:
                    case KeyboardKeys.F9:
                    case KeyboardKeys.F10:
                    case KeyboardKeys.F11:
                    case KeyboardKeys.F12:
                        return Enum.GetName(typeof(KeyboardKeys), keyboardKey);
                    case KeyboardKeys.PrintScreen:
                        if (shortName) { return "PrtSc"; } else { return "Print Screen"; }
                    case KeyboardKeys.ScrollLock:
                        if (shortName) { return "ScLck"; } else { return "Scroll Lock"; }
                    case KeyboardKeys.Pause:
                        return "Pause";
                    case KeyboardKeys.Insert:
                        return "Insert";
                    case KeyboardKeys.Home:
                        return "Home";
                    case KeyboardKeys.PageUp:
                        if (shortName) { return "PgUp"; } else { return "Page Up"; }
                    case KeyboardKeys.Delete:
                        return "Delete";
                    case KeyboardKeys.End:
                        return "End";
                    case KeyboardKeys.PageDown:
                        if (shortName) { return "PgDn"; } else { return "Page Down"; }
                    case KeyboardKeys.ArrowRight:
                        if (shortName) { return "⯈"; } else { return "Arrow Right"; }
                    case KeyboardKeys.ArrowLeft:
                        if (shortName) { return "⯇"; } else { return "Arrow Left"; }
                    case KeyboardKeys.ArrowDown:
                        if (shortName) { return "⯆"; } else { return "Arrow Down"; }
                    case KeyboardKeys.ArrowUp:
                        if (shortName) { return "⯅"; } else { return "Arrow Up"; }
                    case KeyboardKeys.NumLock:
                        if (shortName) { return "NmLck"; } else { return "Num Lock"; }
                    case KeyboardKeys.NumpadSlash:
                        if (shortName) { return "Pad /"; } else { return "Numpad /"; }
                    case KeyboardKeys.NumpadAsterisk:
                        if (shortName) { return "Pad *"; } else { return "Numpad *"; }
                    case KeyboardKeys.NumpadMinus:
                        if (shortName) { return "Pad -"; } else { return "Numpad -"; }
                    case KeyboardKeys.NumpadPlus:
                        if (shortName) { return "Pad +"; } else { return "Numpad +"; }
                    case KeyboardKeys.NumpadEnter:
                        if (shortName) { return "Pad Ent"; } else { return "Numpad Enter"; }
                    case KeyboardKeys.Numpad1:
                        if (shortName) { return "Pad 1"; } else { return "Numpad 1"; }
                    case KeyboardKeys.Numpad2:
                        if (shortName) { return "Pad 2"; } else { return "Numpad 2"; }
                    case KeyboardKeys.Numpad3:
                        if (shortName) { return "Pad 3"; } else { return "Numpad 3"; }
                    case KeyboardKeys.Numpad4:
                        if (shortName) { return "Pad 4"; } else { return "Numpad 4"; }
                    case KeyboardKeys.Numpad5:
                        if (shortName) { return "Pad 5"; } else { return "Numpad 5"; }
                    case KeyboardKeys.Numpad6:
                        if (shortName) { return "Pad 6"; } else { return "Numpad 6"; }
                    case KeyboardKeys.Numpad7:
                        if (shortName) { return "Pad 7"; } else { return "Numpad 7"; }
                    case KeyboardKeys.Numpad8:
                        if (shortName) { return "Pad 8"; } else { return "Numpad 8"; }
                    case KeyboardKeys.Numpad9:
                        if (shortName) { return "Pad 9"; } else { return "Numpad 9"; }
                    case KeyboardKeys.Numpad0:
                        if (shortName) { return "Pad 0"; } else { return "Numpad 0"; }
                    case KeyboardKeys.NumpadPeriod:
                        if (shortName) { return "Pad ."; } else { return "Numpad ."; }
                }
            }
            catch { }
            return Enum.GetName(typeof(KeyboardKeys), keyboardKey);
        }
    }
}