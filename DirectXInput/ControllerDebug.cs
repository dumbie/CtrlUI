using ArnoldVinkCode;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Update controller debug information
        void UpdateControllerDebugInformation(ControllerStatus Controller)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Set basic information
                    textblock_LiveDebugInformation.Text = GenerateControllerDebugString(false);

                    //Clear previous input
                    listbox_LiveDebugInput.Items.Clear();

                    //Get controller input
                    byte[] controllerRawInput = Controller.InputReport;
                    if (controllerRawInput.Length > 200) { controllerRawInput = controllerRawInput.Take(200).ToArray(); }
                    for (int packetId = 0; packetId < controllerRawInput.Length; packetId++)
                    {
                        ProfileShared profileShared = new ProfileShared();
                        profileShared.String1 = packetId.ToString();
                        profileShared.String2 = controllerRawInput[packetId].ToString();
                        listbox_LiveDebugInput.Items.Add(profileShared);
                    }
                });
            }
            catch { }
        }

        //Copy controller debug information
        async void Btn_CopyDebugInformation_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    Clipboard.SetText(GenerateControllerDebugString(true));

                    Debug.WriteLine("Controller debug information copied to clipboard.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Paste";
                    notificationDetails.Text = "Debug information copied";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
                else
                {
                    Debug.WriteLine("Controller debug information is not available.");
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Generate controller debug information string
        string GenerateControllerDebugString(bool includeRawData)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    string rawPackets = "(Out" + activeController.OutputReport.Length + "/In" + activeController.InputReport.Length + ")";
                    if (activeController.Details.Wireless)
                    {
                        rawPackets += "(OffHdWs" + activeController.SupportedCurrent.OffsetWireless + ")";
                    }
                    else
                    {
                        rawPackets += "(OffHdWd" + activeController.SupportedCurrent.OffsetWired + ")";
                    }
                    rawPackets += "(ProductId" + activeController.Details.Profile.ProductID + "/VendorId" + activeController.Details.Profile.VendorID + ")";

                    //Controller raw input
                    if (includeRawData)
                    {
                        rawPackets += "\n";
                        for (int Packet = 0; Packet < activeController.InputReport.Length; Packet++) { rawPackets = rawPackets + " " + activeController.InputReport[Packet]; }
                    }

                    return rawPackets;
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