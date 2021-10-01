using System;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Get the mouse movement amount based on thumb movement (Desktop)
        public static void GetMouseMovementAmountFromThumbDesktop(double thumbSensitivity, int thumbHorizontal, int thumbVertical, bool flipVertical, out int mouseHorizontal, out int mouseVertical)
        {
            mouseHorizontal = 0;
            mouseVertical = 0;
            try
            {
                //Check the thumb movement
                if (flipVertical) { thumbVertical = -thumbVertical; }
                int absHorizontal = Math.Abs(thumbHorizontal);
                int absVertical = Math.Abs(thumbVertical);

                if (absHorizontal > vControllerThumbOffset15000 || absVertical > vControllerThumbOffset15000)
                {
                    double mouseSensitivity = thumbSensitivity / (double)15000;
                    mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                    mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);
                }
                else if (absHorizontal > vControllerThumbOffset2500 || absVertical > vControllerThumbOffset2500)
                {
                    double mouseSensitivity = thumbSensitivity / (double)25000;
                    mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                    mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);
                }
            }
            catch { }
        }

        //Get the mouse movement amount based on thumb movement (Game)
        public static void GetMouseMovementAmountFromThumbGame(double thumbSensitivity, int thumbHorizontal, int thumbVertical, bool flipVertical, out int mouseHorizontal, out int mouseVertical)
        {
            mouseHorizontal = 0;
            mouseVertical = 0;
            try
            {
                //Check the thumb movement
                if (flipVertical) { thumbVertical = -thumbVertical; }
                int absHorizontal = Math.Abs(thumbHorizontal);
                int absVertical = Math.Abs(thumbVertical);

                double mouseSensitivity = thumbSensitivity / (double)15000;
                mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);
            }
            catch { }
        }

        //Get the mouse scroll amount based on thumb movement (Scroll)
        public static void GetMouseMovementAmountFromThumbScroll(int thumbSensitivity, int thumbHorizontal, int thumbVertical, bool flipVertical, out int scrollHorizontal, out int scrollVertical)
        {
            scrollHorizontal = 0;
            scrollVertical = 0;
            try
            {
                //Check the thumb movement
                if (flipVertical) { thumbVertical = -thumbVertical; }

                if (thumbHorizontal > vControllerThumbOffset10000)
                {
                    scrollHorizontal = thumbSensitivity;
                }
                if (thumbVertical > vControllerThumbOffset10000)
                {
                    scrollVertical = thumbSensitivity;
                }

                if (thumbHorizontal < -vControllerThumbOffset10000)
                {
                    scrollHorizontal = -thumbSensitivity;
                }
                if (thumbVertical < -vControllerThumbOffset10000)
                {
                    scrollVertical = -thumbSensitivity;
                }
            }
            catch { }
        }
    }
}