using ArnoldVinkCode;
using System;
using System.Threading;
using static LibraryShared.AppImport;

namespace LibraryShared
{
    public partial class OutputMouse
    {
        //Move the mouse matching the thumb stick
        public static void MouseMovement(int ThumbHorizontal, int ThumbVertical)
        {
            try
            {
                int SmallOffset = 2000;
                int NormalOffset = 15000;
                int MouseHorizontal = 0;
                int MouseVertical = 0;

                ThumbVertical = -ThumbVertical;
                int AbsHorizontal = Math.Abs(ThumbHorizontal);
                int AbsVertical = Math.Abs(ThumbVertical);

                if (AbsHorizontal > NormalOffset || AbsVertical > NormalOffset)
                {
                    double MouseSensitivity = 0.00075;
                    MouseHorizontal = Convert.ToInt32(ThumbHorizontal * MouseSensitivity);
                    MouseVertical = Convert.ToInt32(ThumbVertical * MouseSensitivity);
                }
                else if (AVFunctions.BetweenNumbers(AbsHorizontal, SmallOffset, NormalOffset, true) || AVFunctions.BetweenNumbers(AbsVertical, SmallOffset, NormalOffset, true))
                {
                    double MouseSensitivity = 0.00025;
                    MouseHorizontal = Convert.ToInt32(ThumbHorizontal * MouseSensitivity);
                    MouseVertical = Convert.ToInt32(ThumbVertical * MouseSensitivity);
                }

                //Move cursor to the position
                if (MouseHorizontal != 0 || MouseVertical != 0)
                {
                    GetCursorPos(out PointWin PreviousCursorPosition);
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_MOVE, MouseHorizontal, MouseVertical, 0, IntPtr.Zero);
                }
            }
            catch { }
        }

        //Scroll mouse wheel matching the thumb stick
        public static void MouseWheelScrolling(int ThumbHorizontal, int ThumbVertical)
        {
            try
            {
                //Check the thumb movement
                int SmallOffset = 2000;
                int NormalOffset = 15000;

                int AbsHorizontal = Math.Abs(ThumbHorizontal);
                int AbsVertical = Math.Abs(ThumbVertical);

                if (AbsVertical > SmallOffset && AbsHorizontal < NormalOffset)
                {
                    double MouseSensitivity = 0.0009;
                    int MouseVertical = Convert.ToInt32(ThumbVertical * MouseSensitivity);
                    MouseScrollWheelVertical(MouseVertical);
                }
                else if (AbsHorizontal > SmallOffset && AbsVertical < NormalOffset)
                {
                    double MouseSensitivity = 0.0009;
                    int MouseHorizontal = Convert.ToInt32(ThumbHorizontal * MouseSensitivity);
                    MouseScrollWheelHorizontal(MouseHorizontal);
                }
            }
            catch { }
        }

        //Simulate single key press
        public static void MousePressSingle(bool rightClick)
        {
            try
            {
                if (!rightClick)
                {
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    Thread.Sleep(10);
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                }
                else
                {
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    Thread.Sleep(10);
                    mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                }
            }
            catch { }
        }

        //Simulate mouse up or down
        public static void MouseToggle(bool rightClick, bool downClick)
        {
            try
            {
                if (!rightClick)
                {
                    if (downClick)
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
                    }
                    else
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
                    }
                }
                else
                {
                    if (downClick)
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
                    }
                    else
                    {
                        mouse_event((uint)MouseEvents.MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
                    }
                }
            }
            catch { }
        }

        //Mouse scroll wheel up or down
        public static void MouseScrollWheelVertical(int ScrollAmount)
        {
            try
            {
                mouse_event((uint)MouseEvents.MOUSEEVENTF_VWHEEL, 0, 0, ScrollAmount, IntPtr.Zero);
            }
            catch { }
        }

        //Mouse scroll wheel left and right
        public static void MouseScrollWheelHorizontal(int ScrollAmount)
        {
            try
            {
                mouse_event((uint)MouseEvents.MOUSEEVENTF_HWHEEL, 0, 0, ScrollAmount, IntPtr.Zero);
            }
            catch { }
        }
    }
}