using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;

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

        //Validate the controller
        bool ValidateController(string vendorHexId, string productHexId, string controllerPath)
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
        async Task ConnectController(ControllerDetails ConnectedController)
        {
            try
            {
                //Check if the controller is already in use
                bool ControllerInuse = false;
                if (vController0.Connected() && vController0.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController1.Connected() && vController1.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController2.Connected() && vController2.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (vController3.Connected() && vController3.Details.Path == ConnectedController.Path) { ControllerInuse = true; }
                if (ControllerInuse) { return; }

                Debug.WriteLine("Found a connected " + ConnectedController.Type + " controller to use: " + ConnectedController.DisplayName);

                //Connect the controller to available slot
                if (!vController0.Connected())
                {
                    vController0.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController0);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller0.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Accent.png", UriKind.Relative));
                            textblock_Controller0.Text = vController0.Details.DisplayName;
                        });
                    }
                }
                else if (!vController1.Connected())
                {
                    vController1.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController1);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller1.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Accent.png", UriKind.Relative));
                            textblock_Controller1.Text = vController1.Details.DisplayName;
                        });
                    }
                }
                else if (!vController2.Connected())
                {
                    vController2.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController2);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller2.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Accent.png", UriKind.Relative));
                            textblock_Controller2.Text = vController2.Details.DisplayName;
                        });
                    }
                }
                else if (!vController3.Connected())
                {
                    vController3.Details = ConnectedController;
                    bool controllerStarted = await StartControllerDirectInput(vController3);
                    if (controllerStarted)
                    {
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            image_Controller3.Source = new BitmapImage(new Uri("Assets/Icons/Controller-Accent.png", UriKind.Relative));
                            textblock_Controller3.Text = vController3.Details.DisplayName;
                        });
                    }
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
                if (vController0.Connected() && GetManageController() == null) { SetManageController(vController0); }
                else if (vController1.Connected() && GetManageController() == null) { SetManageController(vController1); }
                else if (vController2.Connected() && GetManageController() == null) { SetManageController(vController2); }
                else if (vController3.Connected() && GetManageController() == null) { SetManageController(vController3); }
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
                if (Controller.Connected() && !Controller.Manage)
                {
                    Debug.WriteLine("Setted the new manage controller to: " + Controller.NumberId);

                    if (GetManageController() != null) { GetManageController().Manage = false; }
                    Controller.Manage = true;

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_ManageControllerName.Text = Controller.Details.DisplayName;
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
                    RawPackets += "(ProductId" + ManageController.Details.Profile.ProductID + "/VendorId" + ManageController.Details.Profile.VendorID + ")";

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