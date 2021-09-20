using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ArnoldVinkCode.AVAudioDevice;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Monitor connected controllers
        async Task MonitorController()
        {
            try
            {
                //Check last controller disconnect time
                double lastDisconnectSeconds = (DateTime.Now - vControllerLastDisconnect).TotalMilliseconds;
                if (lastDisconnectSeconds <= 2500)
                {
                    Debug.WriteLine("A controller disconnected recently, delaying monitor.");
                    return;
                }

                //Load all the connected controllers
                await ControllerReceiveAllConnected();

                //Check if there is an active controller
                await ControllerCheckActivated();
            }
            catch { }
        }

        //Monitor volume mute status
        void MonitorVolumeMute()
        {
            try
            {
                int muteFunction = Convert.ToInt32(Setting_Load(vConfigurationDirectXInput, "ShortcutMuteFunction"));
                if (muteFunction == 0)
                {
                    vControllerMuteLedCurrent = AudioMuteGetStatus(true);
                }
                else
                {
                    vControllerMuteLedCurrent = AudioMuteGetStatus(false);
                }
            }
            catch { }
        }

        //Validate the controller
        bool ControllerValidate(string vendorHexId, string productHexId, string controllerPath, string serialNumber)
        {
            try
            {
                string vendorHexIdLower = vendorHexId.ToLower();
                string productHexIdLower = productHexId.ToLower();

                //Check if the controller is in temp block list
                if (vControllerTempBlockPaths.Contains(controllerPath))
                {
                    Debug.WriteLine("Controller is on temp block list: " + controllerPath);
                    return false;
                }

                //Check if the controller is on user ignore list
                foreach (ControllerIgnored ignoreCheck in vDirectControllersIgnoredUser)
                {
                    string filterVendor = ignoreCheck.VendorID.ToLower();
                    string[] filterProducts = ignoreCheck.ProductIDs.Select(x => x.ToLower()).ToArray();
                    if (filterVendor == vendorHexIdLower && filterProducts.Any(productHexIdLower.Contains))
                    {
                        //Debug.WriteLine("Controller is on user ignore list: " + controllerPath);
                        return false;
                    }
                }

                //Check if the controller is on default ignore list
                foreach (ControllerIgnored ignoreCheck in vDirectControllersIgnoredDefault)
                {
                    string filterVendor = ignoreCheck.VendorID.ToLower();
                    string[] filterProducts = ignoreCheck.ProductIDs.Select(x => x.ToLower()).ToArray();
                    if (filterVendor == vendorHexIdLower && filterProducts.Any(productHexIdLower.Contains))
                    {
                        //Debug.WriteLine("Controller is on default ignore list: " + controllerPath);
                        return false;
                    }
                }

                //Check if controller is already connected by serialnumber
                //if (!string.IsNullOrWhiteSpace(serialNumber))
                //{
                //    //Fix add code that reads serial number from devices
                //}
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
                            image_Controller0.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller0.Text = vController0.Details.DisplayName;
                            textblock_Controller0CodeName.Text = vController0.SupportedCurrent.CodeName;
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
                            image_Controller1.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller1.Text = vController1.Details.DisplayName;
                            textblock_Controller1CodeName.Text = vController1.SupportedCurrent.CodeName;
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
                            image_Controller2.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller2.Text = vController2.Details.DisplayName;
                            textblock_Controller2CodeName.Text = vController2.SupportedCurrent.CodeName;
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
                            image_Controller3.Source = vImagePreloadIconControllerAccent;
                            textblock_Controller3.Text = vController3.Details.DisplayName;
                            textblock_Controller3CodeName.Text = vController3.SupportedCurrent.CodeName;
                        });
                    }
                }
            }
            catch { }
        }

        //Check if there is an actived controller
        async Task ControllerCheckActivated()
        {
            try
            {
                //Debug.WriteLine("There is currently no actived controller.");
                ControllerStatus activeController = vActiveController();
                if (vController0.Connected() && activeController == null) { await ControllerActivate(vController0); }
                else if (vController1.Connected() && activeController == null) { await ControllerActivate(vController1); }
                else if (vController2.Connected() && activeController == null) { await ControllerActivate(vController2); }
                else if (vController3.Connected() && activeController == null) { await ControllerActivate(vController3); }
                else if (activeController == null)
                {
                    //Clear the current controller information
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_ActiveControllerType.Text = "Type";
                        txt_ActiveControllerLatency.Text = "Latency";
                        txt_ActiveControllerBattery.Text = "Battery";
                        txt_ActiveControllerName.Text = "No controller";
                        txt_ActiveControllerName.Foreground = (SolidColorBrush)Application.Current.Resources["ApplicationAccentLightBrush"];
                    });
                }
            }
            catch { }
        }

        //Activate controller
        async Task<bool> ControllerActivate(ControllerStatus Controller)
        {
            try
            {
                if (Controller.Connected() && !Controller.Activated)
                {
                    Debug.WriteLine("Activating controller: " + Controller.NumberId);

                    ControllerStatus activeController = vActiveController();
                    if (activeController != null)
                    {
                        //Deactivate previous controller
                        activeController.Activated = false;

                        //Show controller activated notification
                        string controllerNumberDisplay = (Controller.NumberId + 1).ToString();
                        NotificationDetails notificationDetails = new NotificationDetails();
                        notificationDetails.Icon = "Controller";
                        notificationDetails.Text = "Activated (" + controllerNumberDisplay + ")";
                        notificationDetails.Color = Controller.Color;
                        await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                    }

                    //Activate current controller
                    Controller.Activated = true;

                    //Update settings interface
                    ControllerUpdateSettingsInterface(Controller);
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Change the active controller to 0
        async void Button_Controller0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ControllerActivate(vController0);
            }
            catch { }
        }

        //Change the active controller to 1
        async void Button_Controller1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ControllerActivate(vController1);
            }
            catch { }
        }

        //Change the active controller to 2
        async void Button_Controller2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ControllerActivate(vController2);
            }
            catch { }
        }

        //Change the active controller to 3
        async void Button_Controller3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ControllerActivate(vController3);
            }
            catch { }
        }

        //Save the previous combobox selected item
        void Cb_Controller_Mouse_Down(object sender, EventArgs args)
        {
            try
            {
                ComboBox SelectedComboBox = (ComboBox)sender;
                vComboboxIndexPrev = SelectedComboBox.SelectedIndex;
            }
            catch { }
        }
    }
}