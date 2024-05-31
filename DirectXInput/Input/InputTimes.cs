using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Update button press times
        void CheckControllerButtonPressTimes(ControllerStatus controllerStatus)
        {
            try
            {
                foreach (ControllerButtonDetails buttonDetails in controllerStatus.InputCurrent.Buttons)
                {
                    try
                    {
                        buttonDetails.PressTimeUpdate();
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}