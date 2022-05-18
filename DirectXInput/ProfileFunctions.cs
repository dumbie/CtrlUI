using static ArnoldVinkCode.AVFiles;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public class ProfileFunctions
    {
        public static string GenerateJsonNameControllerProfile(ControllerProfile controllerProfile)
        {
            try
            {
                return @"User\DirectControllersProfile\" + controllerProfile.VendorID.ToLower() + "-" + controllerProfile.ProductID.ToLower();
            }
            catch { }
            return string.Empty;
        }

        public static string GenerateJsonNameKeypadMapping(KeypadMapping keypadMapping)
        {
            try
            {
                return @"User\DirectKeypadMapping\" + FileNameReplaceInvalidChars(keypadMapping.Name.ToLower(), string.Empty);
            }
            catch { }
            return string.Empty;
        }
    }
}