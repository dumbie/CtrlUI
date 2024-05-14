using ArnoldVinkCode.Styles;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVJsonFunctions;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Add text string to list
        void Btn_Settings_KeyboardTextString_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string textString = textbox_Settings_KeyboardTextString.Text;
                string placeholderString = (string)textbox_Settings_KeyboardTextString.GetValue(TextboxPlaceholder.PlaceholderProperty);
                Debug.WriteLine("Adding new text string: " + textString);

                //Color brushes
                BrushConverter BrushConvert = new BrushConverter();
                Brush BrushInvalid = BrushConvert.ConvertFromString("#CD1A2B") as Brush;
                Brush BrushValid = BrushConvert.ConvertFromString("#1DB954") as Brush;

                //Check if the text is empty
                if (string.IsNullOrWhiteSpace(textString))
                {
                    textbox_Settings_KeyboardTextString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter a text string.");
                    return;
                }

                //Check if the text is place holder
                if (textString == placeholderString)
                {
                    textbox_Settings_KeyboardTextString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Please enter a text string.");
                    return;
                }

                //Check if text already exists
                if (vDirectKeyboardTextList.Any(x => x.String1.ToLower() == textString.ToLower()))
                {
                    textbox_Settings_KeyboardTextString.BorderBrush = BrushInvalid;
                    Debug.WriteLine("Text string already exists.");
                    return;
                }

                //Clear text from the textbox
                textbox_Settings_KeyboardTextString.Text = placeholderString;
                textbox_Settings_KeyboardTextString.BorderBrush = BrushValid;

                //Add text string to the list
                ProfileShared profileShared = new ProfileShared();
                profileShared.String1 = textString;

                vDirectKeyboardTextList.Add(profileShared);
                JsonSaveObject(vDirectKeyboardTextList, @"Profiles\User\DirectKeyboardTextList.json");

                //Hide keyboard no text set
                vWindowKeyboard.textblock_TextListNoTextSet.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        //Remove text string from list
        void Btn_Settings_KeyboardTextString_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProfileShared selectedProfile = (ProfileShared)combobox_KeyboardTextString.SelectedItem;
                Debug.WriteLine("Removing text string: " + selectedProfile.String1);

                //Remove mapping from list
                vDirectKeyboardTextList.Remove(selectedProfile);

                //Save changes to Json file
                JsonSaveObject(vDirectKeyboardTextList, @"Profiles\User\DirectKeyboardTextList.json");

                //Select the default profile
                combobox_KeyboardTextString.SelectedIndex = 0;

                //Check if texts are set
                if (!vDirectKeyboardTextList.Any())
                {
                    vWindowKeyboard.textblock_TextListNoTextSet.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }
    }
}