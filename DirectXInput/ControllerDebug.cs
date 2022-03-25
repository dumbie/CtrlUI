using System.Diagnostics;
using System.Linq;
using System.Windows;
using static ArnoldVinkCode.AVActions;
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
                if (vShowDebugInformation && (GetSystemTicksMs() - vShowDebugDelay) > 0)
                {
                    //Set basic information
                    textblock_LiveDebugInformation.Text = GenerateControllerDebugString(false);

                    //Clear previous input
                    listbox_LiveDebugInput.Items.Clear();

                    //Get controller input
                    byte[] controllerRawInput = GetControllerRawInput();
                    if (controllerRawInput.Length > 200) { controllerRawInput = controllerRawInput.Take(200).ToArray(); }
                    for (int packetId = 0; packetId < controllerRawInput.Length; packetId++) 
                    {
                        ProfileShared profileShared = new ProfileShared();
                        profileShared.String1 = packetId.ToString();
                        profileShared.String2 = controllerRawInput[packetId].ToString();
                        listbox_LiveDebugInput.Items.Add(profileShared);
                    }

                    vShowDebugDelay = GetSystemTicksMs() + 100;
                }
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
                    rawPackets += "(OffHd" + activeController.InputHeaderOffsetByte + ")";
                    rawPackets += "(OffBtn" + activeController.InputButtonOffsetByte + ")";
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

        //Get controller raw input
        byte[] GetControllerRawInput()
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    return activeController.InputReport;
                }
            }
            catch { }
            return null;
        }

        //Get controller raw output
        byte[] GetControllerRawOutput()
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    return activeController.OutputReport;
                }
            }
            catch { }
            return null;
        }
    }
}