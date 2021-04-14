using System.Diagnostics;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Update controller debug information
        void UpdateDebugInformation()
        {
            try
            {
                if (vShowDebugInformation)
                {
                    textblock_LiveDebugInformation.Text = GenerateControllerDebugInformation();
                }
            }
            catch { }
        }

        //Copy controller debug information
        void Btn_CopyDebugInformation_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    Clipboard.SetText(GenerateControllerDebugInformation());

                    Debug.WriteLine("Controller debug information copied to clipboard.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Paste";
                    notificationDetails.Text = "Debug information copied";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
                else
                {
                    Debug.WriteLine("Controller debug information is not available.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Generate controller debug information
        string GenerateControllerDebugInformation()
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    string RawPackets = "(Out" + activeController.OutputReport.Length + "/In" + activeController.InputReport.Length + ")";
                    RawPackets += "(OffHd" + activeController.InputHeaderOffsetByte + ")";
                    RawPackets += "(OffBt" + activeController.InputButtonOffsetByte + ")";
                    RawPackets += "(ProductId" + activeController.Details.Profile.ProductID + "/VendorId" + activeController.Details.Profile.VendorID + ")";
                    for (int Packet = 0; Packet < activeController.InputReport.Length; Packet++) { RawPackets = RawPackets + " " + activeController.InputReport[Packet]; }
                    return RawPackets;
                }
                else
                {
                    return "Connect a controller to show debug information.";
                }
            }
            catch { }
            return "Failed to generate debug information.";
        }
    }
}