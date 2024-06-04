using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVJsonFunctions;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Classes;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Close the browser
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hide();
            }
            catch { }
        }

        //Resize the window
        private void Thumb_ResizeGrip_DragDelta(object sender, DragDeltaEventArgs e)
        {
            try
            {
                Width += e.HorizontalChange;
                Height += e.VerticalChange;
            }
            catch { }
        }

        //Move the window
        private void button_Move_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch { }
        }

        //Switch click through mode
        private void button_Pin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Browser_Switch_Clickthrough(false);
            }
            catch { }
        }

        //Go back to previous page
        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vBrowserWebView.GoBack();
            }
            catch { }
        }

        //Refresh the current link
        private void button_LinkRefresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                vBrowserWebView.Reload();
            }
            catch { }
        }

        //Show or hide link menu
        private void button_Link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_Link.Visibility == Visibility.Visible)
                {
                    grid_Link.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid_Link.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Open link from list
        private void listbox_Link_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListBox listboxSender = (ListBox)sender;
                ProfileShared selectedItem = (ProfileShared)listboxSender.SelectedItem;
                Browser_Open_Link(selectedItem.String1, true);
                Debug.WriteLine("Left clicked on link: " + selectedItem.String1);
            }
            catch { }
        }

        //Remove link from list
        private void listbox_Link_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListBox listboxSender = (ListBox)sender;
                ProfileShared selectedItem = (ProfileShared)listboxSender.SelectedItem;
                vFpsBrowserLinks.Remove(selectedItem);
                JsonSaveObject(vFpsBrowserLinks, @"Profiles\User\FpsBrowserLinks.json");
                Debug.WriteLine("Right clicked on link: " + selectedItem.String1);
            }
            catch { }
        }

        //Check link changes
        private void textbox_Link_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string textBoxLink = textbox_Link.Text;
                string currentBrowserLink = vBrowserWebView.Source.ToString();
                if (textBoxLink != currentBrowserLink)
                {
                    button_LinkOpen.Visibility = Visibility.Visible;
                    button_LinkRefresh.Visibility = Visibility.Collapsed;
                }
                else
                {
                    button_LinkOpen.Visibility = Visibility.Collapsed;
                    button_LinkRefresh.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Open link from textbox
        private void textbox_Link_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    TextBox textboxSender = (TextBox)sender;
                    Browser_Open_Link(textboxSender.Text, true);
                    Debug.WriteLine("Entered link: " + textboxSender.Text);
                }
            }
            catch { }
        }

        //Open link from textbox icon
        private async void button_LinkOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textbox_Link.Text))
                {
                    await vWindowMain.Notification_Send_Status("Browser", "Please enter a link");
                    return;
                }

                Browser_Open_Link(textbox_Link.Text, true);
                Debug.WriteLine("Entered link: " + textbox_Link.Text);
            }
            catch { }
        }

        //Update current link text
        private void WebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            try
            {
                textbox_Link.Text = vBrowserWebView.Source.ToString();
            }
            catch { }
        }

        //Copy current link to clipboard
        private async void button_CopyLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string websiteLink = textbox_Link.Text;
                if (!string.IsNullOrWhiteSpace(websiteLink))
                {
                    Clipboard.SetText(websiteLink);
                    await vWindowMain.Notification_Send_Status("Paste", "Link copied to clipboard");
                    Debug.WriteLine("Link copied to clipboard: " + websiteLink);
                }
            }
            catch { }
        }

        //Add current link to link menu
        private async void button_AddLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string websiteLink = textbox_Link.Text;
                if (!string.IsNullOrWhiteSpace(websiteLink))
                {
                    //Check if string is valid link
                    websiteLink = StringLinkCleanup(websiteLink);
                    if (!StringLinkValidate(websiteLink))
                    {
                        await vWindowMain.Notification_Send_Status("Browser", "Invalid link entered");
                        return;
                    }

                    //Check if link already exists
                    if (vFpsBrowserLinks.Any(x => x.String1.ToLower().Replace("/", "") == websiteLink.ToLower().Replace("/", "")))
                    {
                        await vWindowMain.Notification_Send_Status("Browser", "Link already exists");
                        return;
                    }

                    //Add text string to the list
                    ProfileShared profileShared = new ProfileShared();
                    profileShared.String1 = websiteLink;
                    vFpsBrowserLinks.Add(profileShared);
                    JsonSaveObject(vFpsBrowserLinks, @"Profiles\User\FpsBrowserLinks.json");
                    Debug.WriteLine("Link added to menu: " + websiteLink);
                }
            }
            catch { }
        }

        //Open new window link
        private void WebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            try
            {
                e.Handled = true;
                vBrowserWebView.CoreWebView2.Navigate(e.Uri);
            }
            catch { }
        }

        //Cancel downloads
        private void WebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            try
            {
                e.Cancel = true;
            }
            catch { }
        }

        //Update progressbar and opacity
        private async void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                //Update browser opacity
                await Browser_Update_Opacity();

                progressbar_Browser.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        //Update progressbar
        private void WebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            try
            {
                progressbar_Browser.Visibility = Visibility.Visible;
            }
            catch { }
        }
    }
}