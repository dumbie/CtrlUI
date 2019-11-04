using ArnoldVinkCode;
using System;
using System.Configuration;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.ArnoldVinkSocketClass;
using static ArnoldVinkCode.AVClassConverters;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update the controller status icons
        void UpdateControllerConnected()
        {
            try
            {
                //Check if process DirectXInput is running
                if (vProcessDirectXInput == null)
                {
                    //Debug.WriteLine("DirectXInput is not running, skipping controller check.");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Request the controller status from DirectXInput
                RequestControllerStatus("0");
                RequestControllerStatus("1");
                RequestControllerStatus("2");
                RequestControllerStatus("3");
            }
            catch { }
        }

        //Request the controller status from DirectXInput
        void RequestControllerStatus(string ControllerId)
        {
            try
            {
                //Prepare socket data
                SocketSendContainer socketSend = new SocketSendContainer();
                socketSend.SourceIp = vSocketServer.vTcpListenerIp;
                socketSend.SourcePort = vSocketServer.vTcpListenerPort;
                socketSend.Object = new string[] { "ControllerInfo", ControllerId };

                //Request controller status
                byte[] SerializedData = SerializeObjectToBytes(socketSend);

                //Send socket data
                TcpClient socketClient = vSocketClient.SocketClientCheck(vSocketServer.vTcpListenerIp, vSocketServer.vTcpListenerPort + 1, vSocketServer.vTcpListenerTimeout);
                vSocketClient.SocketClientSendBytes(socketClient, SerializedData, vSocketServer.vTcpListenerTimeout, false);
            }
            catch { }
        }

        //Update the controller status from DirectXInput
        void UpdateControllerStatus(ControllerStatusSend ControllerStatusSend)
        {
            try
            {
                //Get the status controller id
                int ControllerIdInt = ControllerStatusSend.NumberId;
                string ControllerIdString = Convert.ToString(ControllerStatusSend.NumberId);
                //Debug.WriteLine("Received controller status for: " + ControllerIdInt);

                //Get the target controller
                Image ImageTarget = null;
                ControllerStatusSend ControllerTarget = null;
                if (ControllerIdInt == 0)
                {
                    ImageTarget = img_Menu_Controller0;
                    ControllerTarget = vController0;
                }
                else if (ControllerIdInt == 1)
                {
                    ImageTarget = img_Menu_Controller1;
                    ControllerTarget = vController1;
                }
                else if (ControllerIdInt == 2)
                {
                    ImageTarget = img_Menu_Controller2;
                    ControllerTarget = vController2;
                }
                else if (ControllerIdInt == 3)
                {
                    ImageTarget = img_Menu_Controller3;
                    ControllerTarget = vController3;
                }

                //Show controller connection popup and update the controller menu images
                if (ControllerTarget.Connected != ControllerStatusSend.Connected)
                {
                    if (ControllerStatusSend.Connected)
                    {
                        AVActions.ActionDispatcherInvoke(delegate { ImageTarget.Opacity = 1.00; });
                        string ControllerIdDisplay = Convert.ToString(ControllerIdInt + 1);
                        Popup_Show_Status("Controller", "Connected (" + ControllerIdDisplay + ")");
                    }
                    else
                    {
                        AVActions.ActionDispatcherInvoke(delegate { ImageTarget.Opacity = 0.30; });
                        string ControllerIdDisplay = Convert.ToString(ControllerIdInt + 1);
                        Popup_Show_Status("Controller", "Disconnected (" + ControllerIdDisplay + ")");
                        if (vControllerActiveId == ControllerIdString) { HideBatteryStatus(true); }
                    }
                }

                //Check if the controller is manage controller
                if (ControllerStatusSend.Manage && vControllerActiveId != ControllerIdString)
                {
                    ActivateController(ControllerIdString);
                }

                //Update the battery icons and level
                if (ControllerStatusSend.Manage)
                {
                    UpdateBatteryStatus(ControllerIdString, ControllerStatusSend.BatteryPercentageCurrent);
                }

                //Update the controller status for comparison
                if (ControllerIdInt == 0)
                {
                    vController0 = ControllerStatusSend;
                }
                else if (ControllerIdInt == 1)
                {
                    vController1 = ControllerStatusSend;
                }
                else if (ControllerIdInt == 2)
                {
                    vController2 = ControllerStatusSend;
                }
                else if (ControllerIdInt == 3)
                {
                    vController3 = ControllerStatusSend;
                }
            }
            catch { }
        }

        //Check if battery setting is enabled
        bool HideBatteryStatus(bool ForceHide)
        {
            try
            {
                if (ForceHide || ConfigurationManager.AppSettings["HideBatteryLevel"] == "True")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return true;
                }
            }
            catch { }
            return false;
        }

        //Update the battery icons and level
        void UpdateBatteryStatus(string ControllerId, int BatteryPercentageCurrent)
        {
            try
            {
                //Check if the battery setting is enabled
                if (HideBatteryStatus(false)) { return; }
                //Debug.WriteLine("Updating battery level of controller: " + ControllerId);

                //Check if battery level is available
                if (BatteryPercentageCurrent == -1)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Time.Visibility = Visibility.Collapsed;
                    });
                    return;
                }

                //Check if battery is charging
                if (BatteryPercentageCurrent == -2)
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        txt_Main_Battery.Visibility = Visibility.Collapsed;
                        img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerCharge.png" }, IntPtr.Zero, -1);
                        img_Main_Battery.Visibility = Visibility.Visible;
                        img_Main_Time.Visibility = Visibility.Visible;
                    });
                    return;
                }

                //Set the battery percentage text
                AVActions.ActionDispatcherInvoke(delegate
                {
                    txt_Main_Battery.Text = Convert.ToString(BatteryPercentageCurrent) + "%";
                    txt_Main_Battery.Visibility = Visibility.Visible;
                });

                //Set the used battery status icon
                AVActions.ActionDispatcherInvoke(delegate
                {
                    if (BatteryPercentageCurrent <= 10) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis10.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 20) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis20.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 30) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis30.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 40) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis40.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 50) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis50.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 60) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis60.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 70) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis70.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 80) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis80.png" }, IntPtr.Zero, -1); }
                    else if (BatteryPercentageCurrent <= 90) { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis90.png" }, IntPtr.Zero, -1); }
                    else { img_Main_Battery.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Battery/BatteryVerDis100.png" }, IntPtr.Zero, -1); }
                    img_Main_Battery.Visibility = Visibility.Visible;
                    img_Main_Time.Visibility = Visibility.Visible;
                });
            }
            catch
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    txt_Main_Battery.Visibility = Visibility.Collapsed;
                    img_Main_Battery.Visibility = Visibility.Collapsed;
                    img_Main_Time.Visibility = Visibility.Collapsed;
                });
            }
        }

        //Set a controller as the active controller
        void ActivateController(string ControllerId)
        {
            try
            {
                if (ControllerId == "0")
                {
                    vControllerActiveId = ControllerId;
                    Popup_Show_Status("Controller", "Activated (1)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                    });
                }
                else if (ControllerId == "1")
                {
                    vControllerActiveId = ControllerId;
                    Popup_Show_Status("Controller", "Activated (2)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                    });
                }
                else if (ControllerId == "2")
                {
                    vControllerActiveId = ControllerId;
                    Popup_Show_Status("Controller", "Activated (3)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                    });
                }
                else if (ControllerId == "3")
                {
                    vControllerActiveId = ControllerId;
                    Popup_Show_Status("Controller", "Activated (4)");
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        img_Menu_Controller0.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller1.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller2.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller.png" }, IntPtr.Zero, -1);
                        img_Menu_Controller3.Source = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Controller-Accent.png" }, IntPtr.Zero, -1);
                    });
                }
            }
            catch { }
        }
    }
}