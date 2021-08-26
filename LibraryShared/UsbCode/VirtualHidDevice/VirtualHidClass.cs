using System;

namespace LibraryUsb
{
    public partial class VirtualHidDevice
    {
        //DDCode key list
        public enum KeysDDCode : int
        {
            BackSpace = 214,
            Tab = 300,
            Enter = 313,
            ShiftLeft = 500,
            ShiftRight = 511,
            ControlLeft = 600,
            ControlRight = 607,
            AltLeft = 602,
            AltRight = 604,
            Pause = 702,
            CapsLock = 400,
            Escape = 100,
            Space = 603,
            Prior = 705,
            Next = 708,
            End = 707,
            Home = 704,
            Left = 710,
            Up = 709,
            Right = 712,
            Down = 711,
            PrintScreen = 700,
            Insert = 703,
            Delete = 706,
            Digit0 = 210,
            Digit1 = 201,
            Digit2 = 202,
            Digit3 = 203,
            Digit4 = 204,
            Digit5 = 205,
            Digit6 = 206,
            Digit7 = 207,
            Digit8 = 208,
            Digit9 = 209,
            A = 401,
            B = 505,
            C = 503,
            D = 403,
            E = 303,
            F = 404,
            G = 405,
            H = 406,
            I = 308,
            J = 407,
            K = 408,
            L = 409,
            M = 507,
            N = 506,
            O = 309,
            P = 310,
            Q = 301,
            R = 304,
            S = 402,
            T = 305,
            U = 307,
            V = 504,
            W = 302,
            X = 502,
            Y = 306,
            Z = 501,
            LeftWindows = 601,
            RightWindows = 605,
            ContextMenu = 606,
            Numpad0 = 800,
            Numpad1 = 801,
            Numpad2 = 802,
            Numpad3 = 803,
            Numpad4 = 804,
            Numpad5 = 805,
            Numpad6 = 806,
            Numpad7 = 807,
            Numpad8 = 808,
            Numpad9 = 809,
            Multiply = 812,
            Add = 814,
            Subtract = 813,
            Decimal = 816,
            Divide = 811,
            F1 = 101,
            F2 = 102,
            F3 = 103,
            F4 = 104,
            F5 = 105,
            F6 = 106,
            F7 = 107,
            F8 = 108,
            F9 = 109,
            F10 = 110,
            F11 = 111,
            F12 = 112,
            NumLock = 810,
            ScrollLock = 701,
            OEMSemicolon = 410,
            OEMPlus = 212,
            OEMComma = 508,
            OEMMinus = 211,
            OEMPeriod = 509,
            OEMQuestion = 510,
            OEMTilde = 200,
            OEMOpenBracket = 311,
            OEMBackslash = 213,
            OEMCloseBracket = 312,
            OEMQuote = 411
        }

        //DDCode key names
        public static string GetDDCodeKeyName(KeysDDCode ddCode, bool shortName)
        {
            try
            {
                switch (ddCode)
                {
                    //Letters
                    case KeysDDCode.A:
                    case KeysDDCode.B:
                    case KeysDDCode.C:
                    case KeysDDCode.D:
                    case KeysDDCode.E:
                    case KeysDDCode.F:
                    case KeysDDCode.G:
                    case KeysDDCode.H:
                    case KeysDDCode.I:
                    case KeysDDCode.J:
                    case KeysDDCode.K:
                    case KeysDDCode.L:
                    case KeysDDCode.M:
                    case KeysDDCode.N:
                    case KeysDDCode.O:
                    case KeysDDCode.P:
                    case KeysDDCode.Q:
                    case KeysDDCode.R:
                    case KeysDDCode.S:
                    case KeysDDCode.T:
                    case KeysDDCode.U:
                    case KeysDDCode.V:
                    case KeysDDCode.W:
                    case KeysDDCode.X:
                    case KeysDDCode.Y:
                    case KeysDDCode.Z:
                        return Enum.GetName(typeof(KeysDDCode), ddCode);

                    //Digits
                    case KeysDDCode.Digit0:
                        return "0";
                    case KeysDDCode.Numpad0:
                        if (shortName) { return "Pad 0"; } else { return "Numpad 0"; }
                    case KeysDDCode.Digit1:
                        return "1";
                    case KeysDDCode.Numpad1:
                        if (shortName) { return "Pad 1"; } else { return "Numpad 1"; }
                    case KeysDDCode.Digit2:
                        return "2";
                    case KeysDDCode.Numpad2:
                        if (shortName) { return "Pad 2"; } else { return "Numpad 2"; }
                    case KeysDDCode.Digit3:
                        return "3";
                    case KeysDDCode.Numpad3:
                        if (shortName) { return "Pad 3"; } else { return "Numpad 3"; }
                    case KeysDDCode.Digit4:
                        return "4";
                    case KeysDDCode.Numpad4:
                        if (shortName) { return "Pad 4"; } else { return "Numpad 4"; }
                    case KeysDDCode.Digit5:
                        return "5";
                    case KeysDDCode.Numpad5:
                        if (shortName) { return "Pad 5"; } else { return "Numpad 5"; }
                    case KeysDDCode.Digit6:
                        return "6";
                    case KeysDDCode.Numpad6:
                        if (shortName) { return "Pad 6"; } else { return "Numpad 6"; }
                    case KeysDDCode.Digit7:
                        return "7";
                    case KeysDDCode.Numpad7:
                        if (shortName) { return "Pad 7"; } else { return "Numpad 7"; }
                    case KeysDDCode.Digit8:
                        return "8";
                    case KeysDDCode.Numpad8:
                        if (shortName) { return "Pad 8"; } else { return "Numpad 8"; }
                    case KeysDDCode.Digit9:
                        return "9";
                    case KeysDDCode.Numpad9:
                        if (shortName) { return "Pad 9"; } else { return "Numpad 9"; }

                    //Numpad
                    case KeysDDCode.Add:
                        if (shortName) { return "Pad +"; } else { return "Numpad +"; }
                    case KeysDDCode.Subtract:
                        if (shortName) { return "Pad -"; } else { return "Numpad -"; }
                    case KeysDDCode.Divide:
                        if (shortName) { return "Pad /"; } else { return "Numpad /"; }
                    case KeysDDCode.Multiply:
                        if (shortName) { return "Pad *"; } else { return "Numpad *"; }
                    case KeysDDCode.Decimal:
                        if (shortName) { return "Pad ."; } else { return "Numpad ."; }
                    case KeysDDCode.Space:
                        if (shortName) { return "Spc"; } else { return "Spacebar"; }

                    //OEM
                    case KeysDDCode.OEMSemicolon:
                        return ";";
                    case KeysDDCode.OEMQuestion:
                        return "?";
                    case KeysDDCode.OEMTilde:
                        return "~";
                    case KeysDDCode.OEMOpenBracket:
                        return "[";
                    case KeysDDCode.OEMCloseBracket:
                        return "]";
                    case KeysDDCode.OEMBackslash:
                        return "\\";
                    case KeysDDCode.OEMQuote:
                        return "'";
                    case KeysDDCode.OEMPlus:
                        return "+";
                    case KeysDDCode.OEMMinus:
                        return "-";
                    case KeysDDCode.OEMComma:
                        return ",";
                    case KeysDDCode.OEMPeriod:
                        return ".";

                    //Function
                    case KeysDDCode.F1:
                    case KeysDDCode.F2:
                    case KeysDDCode.F3:
                    case KeysDDCode.F4:
                    case KeysDDCode.F5:
                    case KeysDDCode.F6:
                    case KeysDDCode.F7:
                    case KeysDDCode.F8:
                    case KeysDDCode.F9:
                    case KeysDDCode.F10:
                    case KeysDDCode.F11:
                    case KeysDDCode.F12:
                        return Enum.GetName(typeof(KeysDDCode), ddCode);

                    //Navigation
                    case KeysDDCode.Up:
                        if (shortName) { return "⯅"; } else { return "Arrow Up"; }
                    case KeysDDCode.Down:
                        if (shortName) { return "⯆"; } else { return "Arrow Down"; }
                    case KeysDDCode.Left:
                        if (shortName) { return "⯇"; } else { return "Arrow Left"; }
                    case KeysDDCode.Right:
                        if (shortName) { return "⯈"; } else { return "Arrow Right"; }
                    case KeysDDCode.Prior:
                        if (shortName) { return "PgUp"; } else { return "Page Up"; }
                    case KeysDDCode.Next:
                        if (shortName) { return "PgDn"; } else { return "Page Down"; }
                    case KeysDDCode.Home:
                        return "Home";
                    case KeysDDCode.End:
                        return "End";

                    //Action
                    case KeysDDCode.BackSpace:
                        if (shortName) { return "Bspc"; } else { return "Backspace"; }
                    case KeysDDCode.Tab:
                        return "Tab";
                    case KeysDDCode.Escape:
                        if (shortName) { return "Esc"; } else { return "Escape"; }
                    case KeysDDCode.Enter:
                        return "Enter";
                    case KeysDDCode.ShiftLeft:
                        if (shortName) { return "LShift"; } else { return "Shift (Left)"; }
                    case KeysDDCode.ShiftRight:
                        if (shortName) { return "RShift"; } else { return "Shift (Right)"; }
                    case KeysDDCode.ControlLeft:
                        if (shortName) { return "LCtrl"; } else { return "Control (Left)"; }
                    case KeysDDCode.ControlRight:
                        if (shortName) { return "RCtrl"; } else { return "Control (Right)"; }
                    case KeysDDCode.AltLeft:
                        if (shortName) { return "LAlt"; } else { return "Alt (Left)"; }
                    case KeysDDCode.AltRight:
                        if (shortName) { return "RAlt"; } else { return "Alt (Right)"; }
                    case KeysDDCode.Pause:
                        return "Pause";
                    case KeysDDCode.CapsLock:
                        if (shortName) { return "Caps"; } else { return "Caps Lock"; }
                    case KeysDDCode.NumLock:
                        if (shortName) { return "NLck"; } else { return "Num Lock"; }
                    case KeysDDCode.ScrollLock:
                        if (shortName) { return "SLck"; } else { return "Scroll Lock"; }
                    case KeysDDCode.PrintScreen:
                        if (shortName) { return "PrtSc"; } else { return "Print Screen"; }
                    case KeysDDCode.LeftWindows:
                        if (shortName) { return "LWin"; } else { return "Windows (Left)"; }
                    case KeysDDCode.RightWindows:
                        if (shortName) { return "RWin"; } else { return "Windows (Right)"; }
                    case KeysDDCode.Insert:
                        return "Insert";
                    case KeysDDCode.Delete:
                        return "Delete";
                    case KeysDDCode.ContextMenu:
                        return "Menu";
                }
            }
            catch { }
            return Enum.GetName(typeof(KeysDDCode), ddCode);
        }
    }
}