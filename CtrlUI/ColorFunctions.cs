using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;
using static CtrlUI.AppVariables;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Available color list
        public uint[] uintColors =
        {
            //Grayscale
            0xFF000000,0xFF444444,0xFF888888,0xFFA9A9A9,0xFFC3C3C3,

            //Colors
            0xFFFFFF00,0xFFFFE135,0xFFF3DB32,0xFFF8DE7E,0xFFADFF2F,0xFFB2CF5D,0xFF9CB651,0xFFC1D881,0xFF10C96F,
            0xFF00FF00,0xFF7FFF00,0xFF32CD32,0xFF00FF7F,0xFF90EE90,0xFF42B54F,
            0xFF3CB371,0xFF2E8B57,0xFF008000,0xFF548F3B,0xFF0B8645,0xFF808000,0xFFFF0000,
            0xFFED2939,0xFF800000,0xFFA52A2A,0xFFB22222,0xFFDC143C,
            0xFFFF8C00,0xFFFFA500,0xFFEDBA47,0xFFD2691E,0xFFFF7F50,0xFFF4A460,
            0xFFD6A572,0xFFC9915F,0xFF744030,0xFF8C4A2E,0xFF572316,0xFF24160F,0xFFB2373F,0xFF9B3F53,
            0xFFCD5C5C,0xFFF08080,0xFFFFB6C1,0xFFFFA07A,0xFFFF1493,
            0xFFFF69B4,0xFFFF00FF,0xFFC71585,0xFF800080,0xFF120B23,0xFF4B0082,
            0xFF8A2BE2,0xFFDA70D6,0xFFDB7093,0xFF76608A,0xFF483D8B,
            0xFF000080,0xFF0000FF,0xFF6495ED,0xFF00C7FF,0xFF00BFFF,
            0xFF1E90FF,0xFFADD8E6,0xFF87CEFA,0xFF006FD5,0xFF7B68EE,
            0xFF6A5ACD,0xFF4169E1,0xFF708090,0xFF4682B4,
            0xFF008080,0xFF217283,0xFF40E0D0,0xFF20B2AA,0xFFA0B3C0,0xFF15B0FC
        };

        //Change application accent color
        void ChangeApplicationAccentColor()
        {
            try
            {
                Debug.WriteLine("Adjusting the application accent color.");

                string colorHexLight = ConfigurationManager.AppSettings["ColorAccentLight"].ToString();
                SolidColorBrush targetSolidColorBrushLight = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;
                SolidColorBrush targetSolidColorBrushDark = new BrushConverter().ConvertFrom(colorHexLight) as SolidColorBrush;
                targetSolidColorBrushDark.Opacity = 0.50;

                App.Current.Resources["ApplicationAccentLightColor"] = targetSolidColorBrushLight.Color;
                App.Current.Resources["ApplicationAccentDarkColor"] = targetSolidColorBrushDark.Color;
                App.Current.Resources["ApplicationAccentLightBrush"] = targetSolidColorBrushLight;
                App.Current.Resources["ApplicationAccentDarkBrush"] = targetSolidColorBrushDark;
            }
            catch { }
        }

        //Hide or show the ColorPicker
        async Task Popup_ShowHide_ColorPicker(bool ForceShow)
        {
            try
            {
                if (vColorPickerOpen)
                {
                    await Popup_Close_Top();
                    return;
                }

                if (ForceShow)
                {
                    await Popup_Close_All();
                }

                //Reset the popup to defaults
                Popup_Reset_ColorPicker();

                //Show the search popup
                PlayInterfaceSound("PopupOpen", false);

                //Save the previous focus element
                Popup_PreviousElementFocus_Save(vColorPickerElementFocus, null);

                //Show the popup
                Popup_Show_Element(grid_Popup_ColorPicker);

                vColorPickerOpen = true;

                //Focus on the file picker listbox
                await ListboxFocusIndex(lb_ColorPicker, false, false, -1);
            }
            catch { }
        }

        //Reset the popup to defaults
        void Popup_Reset_ColorPicker()
        {
            try
            {
                //Clear the current popup list
                List_ColorPicker.Clear();

                //Add colors to the list
                List_ColorPicker.Add((SolidColorBrush)App.Current.Resources["ApplicationAccentLightBrush"]);
                foreach (uint uintColor in uintColors)
                {
                    SolidColorBrush newSolidColorBrush = new SolidColorBrush(Color.FromArgb((byte)(uintColor >> 24), (byte)(uintColor >> 16), (byte)(uintColor >> 8), (byte)(uintColor)));
                    List_ColorPicker.Add(newSolidColorBrush);
                };
            }
            catch { }
        }

        async Task Popup_Close_ColorPicker()
        {
            try
            {
                if (vColorPickerOpen)
                {
                    PlayInterfaceSound("PopupClose", false);

                    //Reset popup variables
                    vColorPickerOpen = false;

                    //Clear the current popup list
                    List_ColorPicker.Clear();

                    //Hide the popup
                    Popup_Hide_Element(grid_Popup_ColorPicker);

                    //Focus on the previous focus element
                    await Popup_PreviousElementFocus_Focus(vColorPickerElementFocus);
                }
            }
            catch { }
        }
    }
}