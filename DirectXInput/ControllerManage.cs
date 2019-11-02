using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static LibraryShared.Classes;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Monitor connected controllers
        async Task MonitorControllers()
        {
            try
            {
                //Load all the connected controllers
                await ReceiveAllControllers();

                //Check if there is a manage controller
                CheckManageController();
            }
            catch { }
        }

        //Auto connect connected controllers
        async Task AutoConnectController(ControllerConnected ConnectedController)
        {
            try
            {
                //Check if the controller is already in use
                bool ControllerInuse = false;
                if (vController0.Connected != null && vController0.Connected.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController1.Connected != null && vController1.Connected.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController2.Connected != null && vController2.Connected.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController3.Connected != null && vController3.Connected.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (ControllerInuse) { return; }

                //Check if the controller is in block list
                if (vControllerBlockedPaths.Contains(ConnectedController.Path)) { return; }

                //Check if the controller has a type
                if (string.IsNullOrWhiteSpace(ConnectedController.Type)) { return; }

                //Allow the controller in HidGuardian
                HidGuardianAllowController(ConnectedController);
                await Task.Delay(500);

                Debug.WriteLine("Found a connected controller to use: " + ConnectedController.DisplayName);

                //Connect the controller to available slot
                if (vController0.Connected == null)
                {
                    vController0.NumberId = 0;
                    vController0.Connected = ConnectedController;
                    await StartControllerDirectInput(vController0);

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_Controller0.Text = vController0.Connected.DisplayName;
                    });
                }
                else if (vController1.Connected == null)
                {
                    vController1.NumberId = 1;
                    vController1.Connected = ConnectedController;
                    await StartControllerDirectInput(vController1);

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_Controller1.Text = vController1.Connected.DisplayName;
                    });
                }
                else if (vController2.Connected == null)
                {
                    vController2.NumberId = 2;
                    vController2.Connected = ConnectedController;
                    await StartControllerDirectInput(vController2);

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_Controller2.Text = vController2.Connected.DisplayName;
                    });
                }
                else if (vController3.Connected == null)
                {
                    vController3.NumberId = 3;
                    vController3.Connected = ConnectedController;
                    await StartControllerDirectInput(vController3);

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        textblock_Controller3.Text = vController3.Connected.DisplayName;
                    });
                }
            }
            catch { }
        }

        //Check if there is a manage controller
        void CheckManageController()
        {
            try
            {
                //Debug.WriteLine("There is currently no manage controller.");
                if (vController0.Connected != null && GetManageController() == null) { SetManageController(vController0); }
                else if (vController1.Connected != null && GetManageController() == null) { SetManageController(vController1); }
                else if (vController2.Connected != null && GetManageController() == null) { SetManageController(vController2); }
                else if (vController3.Connected != null && GetManageController() == null) { SetManageController(vController3); }
                else if (GetManageController() == null)
                {
                    //Debug.WriteLine("No other connected controller found to manage.");
                    //Clear the current controller information
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_ManageControllerLatency.Text = "Latency";
                        txt_ManageControllerBattery.Text = "Battery level";
                        txt_ManageControllerName.Text = "Controller";
                    });
                }
            }
            catch { }
        }

        //Set a new manage controller
        void SetManageController(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected != null && !Controller.Manage)
                {
                    Debug.WriteLine("Setted the new manage controller to: " + Controller.NumberId);

                    if (GetManageController() != null) { GetManageController().Manage = false; }
                    Controller.Manage = true;

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_ManageControllerName.Text = Controller.Connected.DisplayName;
                        UpdateControllerSettingsInterface(Controller);
                    });
                }
            }
            catch { }
        }

        //Change the manager controller to 0
        void Button_Controller0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetManageController(vController0);
            }
            catch { }
        }

        //Change the manager controller to 1
        void Button_Controller1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetManageController(vController1);
            }
            catch { }
        }

        //Change the manager controller to 2
        void Button_Controller2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetManageController(vController2);
            }
            catch { }
        }

        //Change the manager controller to 3
        void Button_Controller3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetManageController(vController3);
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
                ControllerStatus ManageController = GetManageController();
                if (ManageController != null && ManageController.InputReport != null)
                {
                    string RawPackets = "(Out" + ManageController.OutputReport.Length + "/In" + ManageController.InputReport.Length + ")";
                    RawPackets += "(Offset" + ManageController.InputHeaderByteOffset + ")";
                    RawPackets += "(ProductId" + ManageController.Connected.Profile.ProductID + "/VendorId" + ManageController.Connected.Profile.VendorID + ")";

                    for (int Packet = 0; Packet < ManageController.InputReport.Length - ManageController.InputHeaderByteOffset; Packet++) { RawPackets = RawPackets + " " + ManageController.InputReport[Packet + ManageController.InputHeaderByteOffset]; }
                    Clipboard.SetText(RawPackets);

                    Debug.WriteLine("Controller debug information copied to clipboard.");
                }
            }
            catch { }
        }

        //Returns the current manage controller status
        ControllerStatus GetManageController()
        {
            try
            {
                if (vController0.Manage) { return vController0; }
                else if (vController1.Manage) { return vController1; }
                else if (vController2.Manage) { return vController2; }
                else if (vController3.Manage) { return vController3; }
                return null;
            }
            catch { return null; }
        }
    }
}