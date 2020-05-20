using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVImage;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Monitor connected controllers
        async Task ControllerMonitor()
        {
            try
            {
                //Load all the connected controllers
                await ControllerReceiveAllConnected();

                //Check if there is an active controller
                ControllerCheckActivated();
            }
            catch { }
        }

        //Validate the controller
        bool ControllerValidate(string vendorHexId, string productHexId, string controllerPath)
        {
            try
            {
                //Check for invalid or unknown controllers by id
                if (productHexId == "0x0000" && vendorHexId == "0x0000") { return false; } //Unknown
                if (productHexId == "0x028e" && vendorHexId == "0x045e") { return false; } //Xbox 360

                //Check if the controller is in temp block list
                if (vControllerTempBlockPaths.Contains(controllerPath))
                {
                    Debug.WriteLine("Controller is on temp block list: " + controllerPath);
                    return false;
                }

                //Check if the controller is in ignore list
                foreach (ControllerSupported ignoreCheck in vDirectControllersIgnored)
                {
                    string filterVendor = ignoreCheck.VendorID.ToLower();
                    string[] filterProducts = ignoreCheck.ProductIDs.Select(x => x.ToLower()).ToArray();
                    if (filterVendor == vendorHexId.ToLower() && filterProducts.Any(productHexId.ToLower().Contains))
                    {
                        Debug.WriteLine("Controller is on ignore list: " + controllerPath);
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }

        //Connect with the controller
        async Task ControllerConnect(ControllerDetails ConnectedController)
        {
            try
            {
                //Check if the controller is already in use
                bool ControllerInuse = false;
                if (vController0.Connected && vController0.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController1.Connected && vController1.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController2.Connected && vController2.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController3.Connected && vController3.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (ControllerInuse) { return; }

                Debug.WriteLine("Found a connected " + ConnectedController.Type + " controller to use: " + ConnectedController.DisplayName);

                //Connect the controller to available slot
                if (!vController0.Connected)
                {
                    vController0.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController0);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller0.Source = FileToBitmapImage(new string[] { "Assets/Icons/Controller-Accent.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            textblock_Controller0.Text = vController0.Details.DisplayName;
                        });
                    }
                }
                else if (!vController1.Connected)
                {
                    vController1.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController1);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller1.Source = FileToBitmapImage(new string[] { "Assets/Icons/Controller-Accent.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            textblock_Controller1.Text = vController1.Details.DisplayName;
                        });
                    }
                }
                else if (!vController2.Connected)
                {
                    vController2.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController2);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller2.Source = FileToBitmapImage(new string[] { "Assets/Icons/Controller-Accent.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            textblock_Controller2.Text = vController2.Details.DisplayName;
                        });
                    }
                }
                else if (!vController3.Connected)
                {
                    vController3.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController3);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller3.Source = FileToBitmapImage(new string[] { "Assets/Icons/Controller-Accent.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                            textblock_Controller3.Text = vController3.Details.DisplayName;
                        });
                    }
                }
            }
            catch { }
        }

        //Check if there is an actived controller
        void ControllerCheckActivated()
        {
            try
            {
                //Debug.WriteLine("There is currently no actived controller.");
                if (vController0.Connected && GetActiveController() == null) { ControllerActivate(vController0); }
                else if (vController1.Connected && GetActiveController() == null) { ControllerActivate(vController1); }
                else if (vController2.Connected && GetActiveController() == null) { ControllerActivate(vController2); }
                else if (vController3.Connected && GetActiveController() == null) { ControllerActivate(vController3); }
                else if (GetActiveController() == null)
                {
                    //Clear the current controller information
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_ActiveControllerLatency.Text = "Latency";
                        txt_ActiveControllerBattery.Text = "Battery level";
                        txt_ActiveControllerName.Text = "Controller";
                    });
                }
            }
            catch { }
        }

        //Activate controller
        bool ControllerActivate(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected && !Controller.Activated)
                {
                    Debug.WriteLine("Activating controller: " + Controller.NumberId);

                    if (GetActiveController() != null)
                    {
                        //Deactivate previous controller
                        GetActiveController().Activated = false;

                        //Show controller activated notification
                        string controllerNumberDisplay = (Controller.NumberId + 1).ToString();
                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Activated (" + controllerNumberDisplay + ")";
                        App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    }

                    Controller.Activated = true;
                    ControllerUpdateSettingsInterface(Controller);
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Change the active controller to 0
        void Button_Controller0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController0);
            }
            catch { }
        }

        //Change the active controller to 1
        void Button_Controller1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController1);
            }
            catch { }
        }

        //Change the active controller to 2
        void Button_Controller2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController2);
            }
            catch { }
        }

        //Change the active controller to 3
        void Button_Controller3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerActivate(vController3);
            }
            catch { }
        }

        //Save the previous combobox selected item
        void Cb_Controller_Mouse_Down(object sender, EventArgs args)
        {
            try
            {
                ComboBox SelectedComboBox = (ComboBox)sender;
                vPrevComboboxIndex = SelectedComboBox.SelectedIndex;
            }
            catch { }
        }

        //Copy Controller Debug Information
        void Btn_CopyControllerDebugInfo_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                ControllerStatus activeController = GetActiveController();
                if (activeController != null && activeController.InputReport != null)
                {
                    string RawPackets = "(Out" + activeController.OutputReport.Length + "/In" + activeController.InputReport.Length + ")";
                    RawPackets += "(Offset" + activeController.InputHeaderByteOffset + ")";
                    RawPackets += "(ProductId" + activeController.Details.Profile.ProductID + "/VendorId" + activeController.Details.Profile.VendorID + ")";

                    for (int Packet = 0; Packet < activeController.InputReport.Length; Packet++) { RawPackets = RawPackets + " " + activeController.InputReport[Packet]; }
                    Clipboard.SetText(RawPackets);

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
                    notificationDetails.Icon = "Paste";
                    notificationDetails.Text = "No information available";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Returns the active controller status
        ControllerStatus GetActiveController()
        {
            try
            {
                if (vController0.Activated) { return vController0; }
                else if (vController1.Activated) { return vController1; }
                else if (vController2.Activated) { return vController2; }
                else if (vController3.Activated) { return vController3; }
            }
            catch { }
            return null;
        }
    }
}