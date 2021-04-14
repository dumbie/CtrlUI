using AVForms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.JsonFunctions;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Load ignored controllers to list
        void ListboxLoadIgnoredController()
        {
            try
            {
                //Clear current list items
                listbox_ControllerIgnore.Items.Clear();

                //Load ignored controllers
                foreach (ControllerSupported controller in vDirectControllersIgnored)
                {
                    foreach (string productId in controller.ProductIDs)
                    {
                        ProfileShared profileShared = new ProfileShared();
                        profileShared.String1 = controller.VendorID;
                        profileShared.String2 = productId;
                        listbox_ControllerIgnore.Items.Add(profileShared);
                    }
                }

                Debug.WriteLine("Loaded ignored controller list.");
            }
            catch { }
        }

        //Ignore and disconnect controller
        async void btn_IgnoreController_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControllerStatus activeController = vActiveController();
                if (activeController != null)
                {
                    int messageResult = await AVMessageBox.MessageBoxPopup(this, "Do you really want to ignore this controller?", "This will prevent this controller model from been converted to XInput.", "Ignore the controller", "Cancel", "", "");
                    if (messageResult == 1)
                    {
                        string lowerVendorId = activeController.Details.Profile.VendorID.ToLower();
                        string lowerProductId = activeController.Details.Profile.ProductID.ToLower();

                        //Update json profile
                        ControllerSupported existingVendor = vDirectControllersIgnored.Where(x => x.VendorID.ToLower() == lowerVendorId).FirstOrDefault();
                        if (existingVendor != null)
                        {
                            List<string> existingProducts = existingVendor.ProductIDs.ToList();
                            existingProducts.Add(lowerProductId);
                            existingVendor.ProductIDs = existingProducts.ToArray();

                            Debug.WriteLine("Updated controller in ignore list: " + lowerVendorId + "/" + lowerProductId);
                        }
                        else
                        {
                            ControllerSupported newController = new ControllerSupported();
                            newController.VendorID = lowerVendorId;
                            newController.ProductIDs = new string[] { lowerProductId };
                            vDirectControllersIgnored.Add(newController);

                            Debug.WriteLine("Added new controller to ignore list: " + lowerVendorId + "/" + lowerProductId);
                        }

                        //Save json profile
                        JsonSaveObject(vDirectControllersIgnored, "DirectControllersIgnored");

                        //Disconnect the controller
                        await StopControllerAsync(activeController, "ignored");
                    }
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Allow all the ignored controllers
        void Btn_AllowIgnoredControllers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Fix
                ////Allow all the ignored controllers
                //foreach (ControllerProfile profile in vDirectControllersProfile)
                //{
                //    profile.Ignore = false;
                //}

                //Save json profile
                JsonSaveObject(vDirectControllersIgnored, "DirectControllersIgnored");

                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.Icon = "Controller";
                notificationDetails.Text = "Allowed the controller";
                App.vWindowOverlay.Notification_Show_Status(notificationDetails);
            }
            catch { }
        }
    }
}