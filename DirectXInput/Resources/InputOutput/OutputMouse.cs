﻿using System;
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

                if (absHorizontal > vControllerOffsetNormal || absVertical > vControllerOffsetNormal)
                {
                    double mouseSensitivity = thumbSensitivity / (double)15000;
                    mouseHorizontal = Convert.ToInt32(thumbHorizontal * mouseSensitivity);
                    mouseVertical = Convert.ToInt32(thumbVertical * mouseSensitivity);
                }
                else if (absHorizontal > vControllerOffsetSmall || absVertical > vControllerOffsetSmall)
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

                if (thumbHorizontal > vControllerOffsetNormal)
                {
                    scrollHorizontal = thumbSensitivity;
                }
                if (thumbVertical > vControllerOffsetNormal)
                {
                    scrollVertical = thumbSensitivity;
                }

                if (thumbHorizontal < -vControllerOffsetNormal)
                {
                    scrollHorizontal = -thumbSensitivity;
                }
                if (thumbVertical < -vControllerOffsetNormal)
                {
                    scrollVertical = -thumbSensitivity;
                }
            }
            catch { }
        }
    }
}