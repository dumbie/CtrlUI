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
                return @"Profiles\User\DirectControllersProfile\" + controllerProfile.VendorID.ToLower() + "-" + controllerProfile.ProductID.ToLower() + ".json";
            }
            catch { }
            return string.Empty;
        }

        public static string GenerateJsonNameKeypadMapping(KeypadMapping keypadMapping)
        {
            try
            {
                return @"Profiles\User\DirectKeypadMapping\" + FileNameReplaceInvalidChars(keypadMapping.Name.ToLower(), string.Empty) + ".json";
            }
            catch { }
            return string.Empty;
        }
    }
}