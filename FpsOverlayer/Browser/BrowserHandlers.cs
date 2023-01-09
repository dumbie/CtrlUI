using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Close the browser
        private async void button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Browser_Close();
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
                await Browser_Switch_Clickthrough();
            }
            catch { }
        }

        //Go back to previous page
        private void button_Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vWebViewer.GoBack();
            }
            catch { }
        }

        //Update the current link
        private void WebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            try
            {
                textbox_WebUrl.Text = vWebViewer.Source.ToString();
            }
            catch { }
        }

        //Open the entered link
        private void button_Check_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vWebViewer.Source = new UriBuilder(textbox_WebUrl.Text).Uri;
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
                    vWebViewer.Source = new UriBuilder(textbox_WebUrl.Text).Uri;
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
                vWebViewer.Source = new UriBuilder(e.Uri).Uri;
            }
            catch { }
        }
    }
}