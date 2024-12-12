using System;
using System.Diagnostics;
using System.Windows;
using static ArnoldVinkCode.AVCertificate;
using static ArnoldVinkCode.AVFirewall;

namespace DriverInstaller
{
    public partial class WindowMain
    {
        async void Button_Driver_Cleanup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Disable the buttons
                ProgressBarUpdate(5, false);
                ElementEnableDisable(button_Driver_Install, false);
                ElementEnableDisable(button_Driver_Uninstall, false);
                ElementEnableDisable(button_Driver_Cleanup, false);
                ElementEnableDisable(button_Driver_Close, false);

                //Close running controller tools
                ProgressBarUpdate(10, false);
                await CloseControllerTools();

                //Start the CtrlUI cleanup
                ProgressBarUpdate(20, false);
                TextBoxAppend("Cleaning up CtrlUI installation.");

                //Remove application tasks
                ProgressBarUpdate(30, false);
                TextBoxAppend("Cleaning up tasks.");
                RemoveServiceTask("ArnoldVink_CtrlUI");
                RemoveServiceTask("ArnoldVink_DirectXInput");
                RemoveServiceTask("ArnoldVink_FpsOverlayer");
                RemoveServiceTask("ArnoldVink_ScreenCaptureTool");

                //Remove application firewall rule
                ProgressBarUpdate(60, false);
                TextBoxAppend("Cleaning up firewall.");
                Firewall_ExecutableRemove("CtrlUI.exe");
                Firewall_ExecutableRemove("DirectXInput.exe");
                Firewall_ExecutableRemove("FpsOverlayer.exe");
                Firewall_ExecutableRemove("ScreenCaptureTool.exe");

                //Remove application certificate
                ProgressBarUpdate(70, false);
                TextBoxAppend("Cleaning up certificate.");
                UninstallCertificate("Arnold Vink");

                //Remove application shortcuts
                ProgressBarUpdate(90, false);
                TextBoxAppend("Cleaning up shortcuts.");
                RemoveStartupShortcut("CtrlUI.url");
                RemoveStartupShortcut("DirectXInput.url");
                RemoveStartupShortcut("FpsOverlayer.url");
                RemoveStartupShortcut("ScreenCaptureTool.url");

                //Reset progress bar
                ProgressBarUpdate(100, true);
                TextBoxAppend("CtrlUI cleanup completed.");
                TextBoxAppend("--- Manually remove CtrlUI directory ---");

                //Enable the buttons
                ElementEnableDisable(button_Driver_Install, true);
                ElementEnableDisable(button_Driver_Uninstall, true);
                ElementEnableDisable(button_Driver_Cleanup, true);
                ElementEnableDisable(button_Driver_Close, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to cleanup: " + ex.Message);
            }
        }
    }
}