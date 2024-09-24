using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVJsonFunctions;
using static FpsOverlayer.AppVariables;
using static FpsOverlayer.NotificationFunctions;
using static LibraryShared.Classes;

namespace FpsOverlayer
{
    public partial class WindowTools : Window
    {
        //Handle starting point
        private void button_Browser_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    vWindowMousePoint = Mouse.GetPosition(null);
                    vWindowMargin = border_Browser.Margin;
                    vWindowWidth = border_Browser.ActualWidth;
                    vWindowHeight = border_Browser.ActualHeight;
                }
            }
            catch { }
        }

        //Handle resize window
        private void button_BrowserResize_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point mouseOffset = Mouse.GetPosition(null);
                    double differenceX = mouseOffset.X - vWindowMousePoint.X;
                    double differenceY = mouseOffset.Y - vWindowMousePoint.Y;

                    double newWidth = vWindowWidth + differenceX;
                    double newHeight = vWindowHeight + differenceY;
                    if (newWidth > border_Browser.MinWidth)
                    {
                        border_Browser.Width = newWidth;
                    }
                    if (newHeight > border_Browser.MinHeight)
                    {
                        border_Browser.Height = newHeight;
                    }
                }
            }
            catch { }
        }

        //Handle move window
        private void button_BrowserMove_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point mouseOffset = Mouse.GetPosition(null);
                    double differenceX = mouseOffset.X - vWindowMousePoint.X;
                    double differenceY = mouseOffset.Y - vWindowMousePoint.Y;

                    Thickness newMargin = vWindowMargin;
                    newMargin.Left += differenceX;
                    newMargin.Top += differenceY;
                    border_Browser.Margin = newMargin;
                }
            }
            catch { }
        }

        //Go back to previous page
        private void button_Browser_Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vBrowserWebView.GoBack();
            }
            catch { }
        }

        //Refresh the current link
        private void button_Browser_LinkRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vBrowserWebView.Reload();
            }
            catch { }
        }

        //Show or hide manage menu
        private void button_Browser_Link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_Browser_Manage.Visibility == Visibility.Visible)
                {
                    grid_Browser_Manage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid_Browser_Manage.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Open link from list
        private void listbox_Browser_Link_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
        private void listbox_Browser_Link_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
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
        private void textbox_Browser_Link_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string textBoxLink = textbox_Browser_Link.Text;
                string currentBrowserLink = vBrowserWebView.Source.ToString();
                if (textBoxLink != currentBrowserLink)
                {
                    button_Browser_LinkOpen.Visibility = Visibility.Visible;
                    button_Browser_LinkRefresh.Visibility = Visibility.Collapsed;
                }
                else
                {
                    button_Browser_LinkOpen.Visibility = Visibility.Collapsed;
                    button_Browser_LinkRefresh.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Open link from textbox
        private void textbox_Browser_Link_PreviewKeyUp(object sender, KeyEventArgs e)
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
        private async void button_Browser_LinkOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textbox_Browser_Link.Text))
                {
                    await Notification_Send_Status("Browser", "Please enter a link");
                    return;
                }

                Browser_Open_Link(textbox_Browser_Link.Text, true);
                Debug.WriteLine("Entered link: " + textbox_Browser_Link.Text);
            }
            catch { }
        }

        //Update current link text
        private void WebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            try
            {
                textbox_Browser_Link.Text = vBrowserWebView.Source.ToString();
            }
            catch { }
        }

        //Copy current link to clipboard
        private async void button_Browser_CopyLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string websiteLink = textbox_Browser_Link.Text;
                if (!string.IsNullOrWhiteSpace(websiteLink))
                {
                    Clipboard.SetText(websiteLink);
                    await Notification_Send_Status("Paste", "Link copied to clipboard");
                    Debug.WriteLine("Link copied to clipboard: " + websiteLink);
                }
            }
            catch { }
        }

        //Add current link to link menu
        private async void button_Browser_AddLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string websiteLink = textbox_Browser_Link.Text;
                if (!string.IsNullOrWhiteSpace(websiteLink))
                {
                    //Check if string is valid link
                    websiteLink = StringLinkCleanup(websiteLink);
                    if (!StringLinkValidate(websiteLink))
                    {
                        await Notification_Send_Status("Browser", "Invalid link entered");
                        return;
                    }

                    //Check if link already exists
                    if (vFpsBrowserLinks.Any(x => x.String1.ToLower().Replace("/", "") == websiteLink.ToLower().Replace("/", "")))
                    {
                        await Notification_Send_Status("Browser", "Link already exists");
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