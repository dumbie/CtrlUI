using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

                //Check ignored controllers
                if (vDirectControllersIgnored.Any())
                {
                    listbox_ControllerIgnore.Visibility = Visibility.Visible;
                    textblock_ControllerIgnore.Visibility = Visibility.Collapsed;
                }
                else
                {
                    listbox_ControllerIgnore.Visibility = Visibility.Collapsed;
                    textblock_ControllerIgnore.Visibility = Visibility.Visible;
                    return;
                }

                //Load ignored controllers
                foreach (ControllerIgnored controllerIgnored in vDirectControllersIgnored)
                {
                    foreach (string productId in controllerIgnored.ProductIDs)
                    {
                        try
                        {
                            ProfileShared profileShared = new ProfileShared();
                            profileShared.String1 = controllerIgnored.CodeName;
                            profileShared.String2 = controllerIgnored.VendorID;
                            profileShared.String3 = productId;
                            profileShared.Object1 = controllerIgnored;
                            listbox_ControllerIgnore.Items.Add(profileShared);
                        }
                        catch { }
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
                    List<string> messageAnswers = new List<string>();
                    messageAnswers.Add("Ignore this controller model");
                    messageAnswers.Add("Cancel");

                    string messageResult = await new AVMessageBox().Popup(this, "Do you really want to ignore this controller?", "This will prevent this controller model from been converted to XInput.", messageAnswers);
                    if (messageResult == "Ignore this controller model")
                    {
                        string lowerVendorId = activeController.Details.Profile.VendorID.ToLower();
                        string lowerProductId = activeController.Details.Profile.ProductID.ToLower();

                        //Update json profile
                        ControllerIgnored existingVendor = vDirectControllersIgnored.Where(x => x.VendorID.ToLower() == lowerVendorId).FirstOrDefault();
                        if (existingVendor != null)
                        {
                            List<string> existingProducts = existingVendor.ProductIDs.ToList();
                            existingProducts.Add(lowerProductId);
                            existingVendor.ProductIDs = existingProducts.ToArray();

                            Debug.WriteLine("Updated controller in ignore list: " + lowerVendorId + "/" + lowerProductId);
                        }
                        else
                        {
                            ControllerIgnored newController = new ControllerIgnored();
                            newController.CodeName = activeController.Details.DisplayName;
                            newController.VendorID = lowerVendorId;
                            newController.ProductIDs = new string[] { lowerProductId };
                            vDirectControllersIgnored.Add(newController);

                            Debug.WriteLine("Added new controller to ignore list: " + lowerVendorId + "/" + lowerProductId);
                        }

                        //Save json profile
                        JsonSaveObject(vDirectControllersIgnored, @"User\DirectControllersIgnored");

                        //Load ignored controllers to list
                        ListboxLoadIgnoredController();

                        //Disconnect the controller
                        StopControllerTask(activeController, "ignored", "Controller " + activeController.Details.DisplayName + " ignored and disconnected.");
                    }
                }
                else
                {
                    NotificationDetails notificationDetails = new NotificationDetails();
                    notificationDetails.Icon = "Controller";
                    notificationDetails.Text = "No controller connected";
                    await App.vWindowOverlay.Notification_Show_Status(notificationDetails);
                }
            }
            catch { }
        }

        //Allow the ignored controller
        async Task AllowIgnoredController()
        {
            try
            {
                ProfileShared selectedItem = (ProfileShared)listbox_ControllerIgnore.SelectedItem;
                ControllerIgnored allowController = (ControllerIgnored)selectedItem.Object1;

                //Update json profile
                List<string> existingProducts = allowController.ProductIDs.ToList();
                existingProducts.Remove(selectedItem.String3);

                //Check empty vendor
                if (existingProducts.Any())
                {
                    allowController.ProductIDs = existingProducts.ToArray();
                }
                else
                {
                    vDirectControllersIgnored.Remove(allowController);
                }

                Debug.WriteLine("Allowed controller in ignore list: " + selectedItem.String2 + "/" + selectedItem.String3);

                //Save json profile
                JsonSaveObject(vDirectControllersIgnored, @"User\DirectControllersIgnored");

                //Load ignored controllers to list
                ListboxLoadIgnoredController();

                //Notify user controller is allowed
                NotificationDetails notificationDetailsAllowed = new NotificationDetails();
                notificationDetailsAllowed.Icon = "Controller";
                notificationDetailsAllowed.Text = "Allowed the controller";
                await App.vWindowOverlay.Notification_Show_Status(notificationDetailsAllowed);
            }
            catch { }
        }

        async void Listbox_ControllerIgnore_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVFunctions.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    vSingleTappedEvent = true;
                    await Task.Delay(500);
                    if (vSingleTappedEvent) { await AllowIgnoredController(); }
                }
            }
            catch { }
        }

        async void Listbox_ControllerIgnore_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space) { await AllowIgnoredController(); }
            }
            catch { }
        }
    }
}