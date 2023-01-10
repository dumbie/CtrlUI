using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;
using static LibraryShared.FocusFunctions;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Close the browser
        private async void button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Hide();
            }
            catch { }
        }

        //Resize the window
        private void ResizeGripper_DragDelta(object sender, DragDeltaEventArgs e)
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
        private async void button_Pin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Browser_Switch_Clickthrough(false);
            }
            catch { }
        }

        //Go back to previous page
        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                webview_Browser.GoBack();
            }
            catch { }
        }

        //Refresh the page
        private void button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                webview_Browser.Reload();
            }
            catch { }
        }

        //Update the current link
        private void WebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            try
            {
                textbox_WebUrl.Text = webview_Browser.Source.ToString();
            }
            catch { }
        }

        //Open link popup
        private async void button_Link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_Link.Visibility == Visibility.Visible)
                {
                    grid_Link.Visibility = Visibility.Collapsed;

                    //Update the window style
                    vBrowserWindowNoActivate = true;
                    await UpdateWindowStyleVisible(vInteropWindowHandle, true, vBrowserWindowNoActivate, vBrowserWindowClickThrough);
                }
                else
                {
                    grid_Link.Visibility = Visibility.Visible;

                    //Update the window style
                    vBrowserWindowNoActivate = false;
                    await UpdateWindowStyleVisible(vInteropWindowHandle, true, vBrowserWindowNoActivate, vBrowserWindowClickThrough);

                    //Focus on the textbox
                    await FrameworkElementFocus(textbox_WebUrl, false, vInteropWindowHandle);
                }
            }
            catch { }
        }

        //Open the entered link
        private void button_Check_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Browser_Open_Link(textbox_WebUrl.Text);
            }
            catch { }
        }

        //Open the entered link
        private void textbox_WebUrl_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    Browser_Open_Link(textbox_WebUrl.Text);
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
                webview_Browser.Source = new UriBuilder(e.Uri).Uri;
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

        //Update progressbar
        private void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
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