using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DirectXInput.AppVariables;
using static LibraryShared.SoundPlayer;

namespace DirectXInput.KeyboardCode
{
    partial class WindowKeyboard
    {
        //Handle get and lost focus
        private void key_Tool_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Button senderButton = (Button)sender;
                //Debug.WriteLine("Tool key selected: " + senderButton);

                if (senderButton == key_Tool_Close)
                {
                    textblock_Tool_Help.Text = "Close keyboard overlay";
                }
                else if (senderButton == key_Tool_SwitchMode)
                {
                    textblock_Tool_Help.Text = "Switch keyboard mode";
                }
                else if (senderButton == key_Tool_Keypad)
                {
                    textblock_Tool_Help.Text = "Show keypad overlay";
                }
                else if (senderButton == key_Tool_CtrlUILaunch)
                {
                    textblock_Tool_Help.Text = "Launch or show CtrlUI";
                }
                else if (senderButton == key_Tool_FpsOverlayerLaunch)
                {
                    textblock_Tool_Help.Text = "Show or hide Fps Overlayer";
                }
                else if (senderButton == key_Tool_FpsOverlayerPosition)
                {
                    textblock_Tool_Help.Text = "Change Fps Overlayer position";
                }
                else if (senderButton == key_Tool_FpsTools)
                {
                    textblock_Tool_Help.Text = "Show or hide tools overlay";
                }
                else if (senderButton == key_Tool_FpsCrosshair)
                {
                    textblock_Tool_Help.Text = "Show or hide crosshair overlay";
                }
            }
            catch { }
        }
        private void key_Tool_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                textblock_Tool_Help.Text = string.Empty;
                //Debug.WriteLine("Tool key unselected.");
            }
            catch { }
        }

        //Hande tool button click
        private async void key_Tool_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await Tool_ExecuteAction(sender);
                }
            }
            catch { }
        }
        private async void key_Tool_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await Tool_ExecuteAction(sender);
            }
            catch { }
        }

        //Execute tool action
        async Task Tool_ExecuteAction(object sender)
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "Click", false, false);

                Button senderButton = (Button)sender;
                if (senderButton == key_Tool_Close)
                {
                    await this.Hide();
                }
                else if (senderButton == key_Tool_SwitchMode)
                {
                    await SwitchKeyboardMode();
                }
                else if (senderButton == key_Tool_Keypad)
                {
                    await vWindowMain.KeypadPopupHideShow(true);
                }
                else if (senderButton == key_Tool_CtrlUILaunch)
                {
                    await ToolFunctions.CtrlUI_LaunchShow();
                }
                else if (senderButton == key_Tool_FpsOverlayerLaunch)
                {
                    await ToolFunctions.FpsOverlayer_LaunchShowHide();
                }
                else if (senderButton == key_Tool_FpsOverlayerPosition)
                {
                    await ToolFunctions.FpsOverlayer_ChangePosition();
                }
                else if (senderButton == key_Tool_FpsTools)
                {
                    await ToolFunctions.FpsOverlayer_ShowHideTools();
                }
                else if (senderButton == key_Tool_FpsCrosshair)
                {
                    await ToolFunctions.FpsOverlayer_ShowHideCrosshair();
                }
            }
            catch { }
        }
    }
}