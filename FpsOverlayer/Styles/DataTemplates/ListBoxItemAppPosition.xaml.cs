using FpsOverlayer;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static LibraryShared.Classes;

namespace ArnoldVinkCode.Styles
{
    public partial class ListBoxItemAppPosition : ResourceDictionary
    {
        void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Button senderButton = sender as Button;
                ProfileShared FpsPositionProcessName = senderButton.DataContext as ProfileShared;

                Debug.WriteLine("Removing application: " + FpsPositionProcessName.String1);
                AppVariables.vFpsPositionProcessName.Remove(FpsPositionProcessName);
                JsonFunctions.JsonSaveObject(AppVariables.vFpsPositionProcessName, "FpsPositionProcessName");
            }
            catch { }
        }

        void Combobox_TextPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox senderComboBox = sender as ComboBox;
                ProfileShared FpsPositionProcessName = senderComboBox.DataContext as ProfileShared;

                Debug.WriteLine("Position changed to: " + senderComboBox.SelectedIndex + " for " + FpsPositionProcessName.String1);
                FpsPositionProcessName.Int1 = senderComboBox.SelectedIndex;
                JsonFunctions.JsonSaveObject(AppVariables.vFpsPositionProcessName, "FpsPositionProcessName");
            }
            catch { }
        }
    }
}