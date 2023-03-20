using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

        //Refresh the page
        private void button_Refresh_Click(object sender, RoutedEventArgs e)
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
        private void listbox_Link_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListBox listboxSender = (ListBox)sender;
                ProfileShared selectedItem = (ProfileShared)listboxSender.SelectedItem;
                Debug.WriteLine("Clicked on link: " + selectedItem.String1);
                Browser_Open_Link(selectedItem.String1, true);
            }
            catch { }
        }

        //Update current link text
        private void WebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            try
            {
                textblock_Link.Text = vBrowserWebView.Source.ToString();
            }
            catch { }
        }

        //Copy current link to clipboard
        private void button_CopyLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textblock_Link.Text != "Current website link")
                {
                    Clipboard.SetText(textblock_Link.Text);
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