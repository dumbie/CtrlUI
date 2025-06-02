using System;
using System.Diagnostics;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        private static bool InputUpdateIdentifiers(ControllerStatus controllerStatus)
        {
            try
            {
                //Check if output number is already set
                if (controllerStatus.NumberOutput != -1) { return true; }

                if (controllerStatus.SupportedCurrent.CodeName == "SonyPS12DualShock")
                {
                    if (controllerStatus.SupportedCurrent.OffsetHeader.NumberOutput != null)
                    {
                        byte identifierByte = controllerStatus.ControllerDataInput[(int)controllerStatus.SupportedCurrent.OffsetHeader.NumberOutput];
                        controllerStatus.NumberOutput = identifierByte;
                    }
                }

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get controller identifiers: " + ex.Message);
                return false;
            }
        }
    }
}