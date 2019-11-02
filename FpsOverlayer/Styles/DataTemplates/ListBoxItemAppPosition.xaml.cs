using FpsOverlayer;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static LibraryShared.Classes;

namespace CtrlUI.Styles
{
    public partial class ListBoxItemAppPosition : ResourceDictionary
    {
        void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Button senderButton = sender as Button;
                FpsPositionProcess fpsPositionProcess = senderButton.DataContext as FpsPositionProcess;

                Debug.WriteLine("Removing application: " + fpsPositionProcess.Process);
                AppVariables.vFpsPositionProcess.Remove(fpsPositionProcess);
                JsonFunctions.JsonSaveFpsPositionProcess();
            }
            catch { }
        }

        void Combobox_TextPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox senderComboBox = sender as ComboBox;
                FpsPositionProcess fpsPositionProcess = senderComboBox.DataContext as FpsPositionProcess;

                Debug.WriteLine("Position changed to: " + senderComboBox.SelectedIndex + " for " + fpsPositionProcess.Process);
                fpsPositionProcess.Position = senderComboBox.SelectedIndex;
                JsonFunctions.JsonSaveFpsPositionProcess();
            }
            catch { }
        }
    }
}